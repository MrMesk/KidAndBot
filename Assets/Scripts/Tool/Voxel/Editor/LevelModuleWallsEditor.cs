using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelModule
{

    [CustomEditor(typeof(LevelModuleWalls))]
    [CanEditMultipleObjects]
    public class LevelModuleWallsEditor : Editor
    {

        /********
         * DATA *
         ********/

        // Target

        /// <summary>
        /// The target level module wall this editor is currently editing.
        /// </summary>
        LevelModuleWalls t { get { return target as LevelModuleWalls; } }



        // Selection

        /// <summary>
        /// The type of selection array the user can use.
        /// </summary>
        public enum SelectionMode
        {
            NONE,
            FILL,
            ERASE
        }

        /// <summary>
        /// The curent type of selection the user is currently performing.
        /// </summary>
        SelectionMode selectionMode = SelectionMode.NONE;

        /// <summary>
        /// The curent selection start and end position.
        /// </summary>
        Vector3? selectionStartPos = null;
        Vector3? selectionEndPos = null;

        /// <summary>
        /// The curent type of selection the user is currently performing.
        /// </summary>
        Material selectionFillMaterial;
        Material selectionEraseMaterial;




        /****************
         * UNITY EDITOR *
         ****************/

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public void OnEnable()
        {
            LevelModuleWalls levelModuleWalls = t;
            levelModuleWalls.InitialiseIfNeeded();
            levelModuleWalls.GenerateMesh();

            Undo.undoRedoPerformed += OnUndoRedo;

            // Setup selection draw materials
            Color matColor;
            // Fill material
            selectionFillMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            matColor = Color.cyan;
            matColor.a = 0.25f;
            selectionFillMaterial.SetColor("_TintColor", matColor);
            selectionFillMaterial.SetFloat("_SoftParticlesFactor", 10);
            // Erase material
            selectionEraseMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            matColor = Color.red;
            matColor.a = 0.25f;
            selectionEraseMaterial.SetColor("_TintColor", matColor);
            selectionEraseMaterial.SetFloat("_SoftParticlesFactor", 10);

        }

        /// <summary>
        /// This function is called when the user performs either an undo or a redo. (CTRL+Z / CTRL+Y)
        /// </summary>
        public void OnUndoRedo()
        {
            LevelModuleWalls levelModuleWalls = t;
            if (levelModuleWalls != null)
            {
                levelModuleWalls.GenerateMesh();
            }
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Retrieve targeted LevelModuleWalls
            LevelModuleWalls levelModuleWalls = t;

            DrawDefaultInspector();

            GUILayout.Label("Level Module Walls");
            // Buton used to reset a level module.
            if (GUILayout.Button("Reset"))
            {
                // Save the current object state to allow an undo/redo.
                Undo.RecordObject(levelModuleWalls, string.Format("Reset voxels from {0}", levelModuleWalls.name));
                // Reinitialise this level-module.
                levelModuleWalls.Init();
                // Refresh it's mesh.
                levelModuleWalls.RefreshMesh();
            }
        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        private void OnSceneGUI()
        {
            LevelModuleWalls levelModuleWalls = (LevelModuleWalls)target;

            // Update the selection by listening to user inputs.
            UpdateSelection();

            // Draw the selection
            DrawSelection();
        }





        /****************
         * EDITOR LOGIC *
         ****************/

        public void UpdateSelection()
        {
            // Retrieve targeted LevelModuleWalls
            LevelModuleWalls levelModuleWalls = t;

            // Check if the user has his mouse over a scene window
            EditorWindow sceneEditorWindow = SceneView.mouseOverWindow;
            if (sceneEditorWindow == null)
                return;

            // Get the position of the user's mouse on the scene window
            Vector2 mousePositionOnScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);   // Get the mouse position on screen
            Vector2 sceneWindowPositionOnScreen = sceneEditorWindow.position.min;                       // Get the scene window position on screen
            Vector2 mousePositionOnSceneWindow = mousePositionOnScreen - sceneWindowPositionOnScreen;   // Calculate the mouse position on scene window

            // Make a ray from the scene camera
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePositionOnSceneWindow);

            // Check if the mouse is over a <LevelModuleWalls>
            RaycastHit hit;
            if (
                Physics.Raycast(ray, out hit) &&                                    // Check if the raycast hit an object
                hit.collider.gameObject.GetComponent<LevelModuleWalls>() != null    // Check if the object hit is a <LevelModuleWalls>
            )
            {

                // DEBUG - Draw a sphere at the hit point found position
                Handles.SphereCap(0, hit.point, Quaternion.identity, 0.01f);

                /// [Check for selection array start.]
                if (!selectionStartPos.HasValue)
                {

                    // Start point isn't defined

                    // Get local hit point
                    Vector3 localHitPoint = TransformUtility.FromWorldToLocalPosition(hit.collider.transform, hit.point);

                    // Check for left click down
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.shift)
                    {
                        Event.current.Use();
                        // Set selction array start point
                        selectionStartPos = hit.point;
                        selectionStartPos += hit.normal * (0.25f / LevelModuleWalls.BlockSubdivisions);
                        // Set selection array as an add array
                        selectionMode = SelectionMode.FILL;
                    }
                    else
                    // Check for right click down
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift)
                    {
                        Event.current.Use();
                        // Set selction array start point
                        selectionStartPos = hit.point;
                        selectionStartPos -= hit.normal * (0.25f / LevelModuleWalls.BlockSubdivisions);
                        // Set selection array as a remove array
                        selectionMode = SelectionMode.ERASE;
                    }
                }
                else
                {

                    // Selection start point is defined

                    /// [Refresh selection end point.]
                    selectionEndPos = hit.point;
                    switch (selectionMode)
                    {
                        case SelectionMode.FILL:
                            // End selection in front of the voxel if filling.
                            selectionEndPos += hit.normal * (0.25f / LevelModuleWalls.BlockSubdivisions);
                            break;
                        case SelectionMode.ERASE:
                            // End selection inside the voxel if erasing.
                            selectionEndPos -= hit.normal * (0.25f / LevelModuleWalls.BlockSubdivisions);
                            break;
                    }
                }
            }

            // Check if a selection is active
            if (selectionMode != SelectionMode.NONE && selectionStartPos.HasValue && selectionEndPos.HasValue)
            {
                // Destroy selection if the user release the input associated 

                // Get selection's min/max
                Vector3 localStartPoint = TransformUtility.FromWorldToLocalPosition(levelModuleWalls.transform, selectionStartPos.Value);
                Vector3 localEndPoint = TransformUtility.FromWorldToLocalPosition(levelModuleWalls.transform, selectionEndPos.Value);
                Vector3 min = new Vector3(
                    Mathf.Min(localStartPoint.x, localEndPoint.x),
                    Mathf.Min(localStartPoint.y, localEndPoint.y),
                    Mathf.Min(localStartPoint.z, localEndPoint.z)
                    );
                min = MathUtility.RaseterizeFloor(min, steps: LevelModuleWalls.BlockSubdivisions);
                Vector3 max = new Vector3(
                    Mathf.Max(localStartPoint.x, localEndPoint.x),
                    Mathf.Max(localStartPoint.y, localEndPoint.y),
                    Mathf.Max(localStartPoint.z, localEndPoint.z)
                    );
                max = MathUtility.RaseterizeCeil(max, steps: LevelModuleWalls.BlockSubdivisions);

                min += Vector3.one * 0.5f;
                max += Vector3.one * 0.5f;
                min *= LevelModuleWalls.BlockSubdivisions;
                max *= LevelModuleWalls.BlockSubdivisions;

                // Check for left click up
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && selectionMode == SelectionMode.FILL)
                {
                    Event.current.Use();

                    // Add voxels
                    Undo.RecordObject(levelModuleWalls, string.Format("Add voxels from {0}", levelModuleWalls.name));
                    levelModuleWalls.Edit_FillArray(min, max);
                    levelModuleWalls.RefreshMesh();

                    // DEBUG
                    levelModuleWalls.colorDebug = Color.green;

                    // Remove selction array start and end points
                    selectionStartPos = null;
                    selectionEndPos = null;
                    // Set selection array as non existent
                    selectionMode = SelectionMode.NONE;
                }
                else
                // Check for left click down
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && selectionMode == SelectionMode.ERASE)
                {
                    Event.current.Use();

                    // Remove voxels
                    Undo.RecordObject(levelModuleWalls, string.Format("Removed voxels from {0}", levelModuleWalls.name));
                    levelModuleWalls.Edit_EraseArray(min, max);
                    levelModuleWalls.RefreshMesh();

                    // DEBUG
                    levelModuleWalls.colorDebug = Color.red;

                    // Remove selction array start and end points
                    selectionStartPos = null;
                    selectionEndPos = null;
                    // Set selection array as non existent
                    selectionMode = SelectionMode.NONE;
                }
            }
        }




        /********************
         * EDITOR RENDERING *
         ********************/

        public void DrawSelection()
        {
            // Retrieve targeted LevelModuleWalls
            LevelModuleWalls levelModuleWalls = t;

            // DEBUG - Two points
            Color handlesColorBackup = Handles.color; // Backup handles color
            if (selectionStartPos.HasValue)
            {
                // DEBUG - Draw a sphere at the start point position
                Handles.color = Color.red;
                Handles.SphereCap(0, selectionStartPos.Value, Quaternion.identity, 0.05f);
            }
            if (selectionEndPos.HasValue)
            {
                // DEBUG - Draw a sphere at the start point position
                Handles.color = Color.blue;
                Handles.SphereCap(0, selectionEndPos.Value, Quaternion.identity, 0.05f);
            }

            // Draw selection if it exist
            if (selectionStartPos.HasValue && selectionEndPos.HasValue)
            {
                Vector3 selectionCenter = Vector3.Lerp(selectionStartPos.Value, selectionEndPos.Value, 0.5f);
                Handles.color = new Color32(0, 255, 255, 128);

                // Config
                const float selectionOffset = 0.05f / LevelModuleWalls.BlockSubdivisions; 

                // Convert start and end point to 
                Vector3 localStartPoint = TransformUtility.FromWorldToLocalPosition(levelModuleWalls.transform, selectionStartPos.Value);
                Vector3 localEndPoint = TransformUtility.FromWorldToLocalPosition(levelModuleWalls.transform, selectionEndPos.Value);

                // Get selection min position
                Vector3 min = new Vector3(
                    Mathf.Min(localStartPoint.x, localEndPoint.x),
                    Mathf.Min(localStartPoint.y, localEndPoint.y),
                    Mathf.Min(localStartPoint.z, localEndPoint.z)
                    );
                min = MathUtility.RaseterizeFloor(min, steps: LevelModuleWalls.BlockSubdivisions);
                min -= Vector3.one * selectionOffset;

                // Get selection max position
                Vector3 max = new Vector3(
                    Mathf.Max(localStartPoint.x, localEndPoint.x),
                    Mathf.Max(localStartPoint.y, localEndPoint.y),
                    Mathf.Max(localStartPoint.z, localEndPoint.z)
                    );
                max = MathUtility.RaseterizeCeil(max, steps: LevelModuleWalls.BlockSubdivisions);
                max += Vector3.one * selectionOffset;

                // Generate selction mesh
                Mesh mesh = GenerateSelectionMesh(min, max);

                // Position mesh in world
                Matrix4x4 matrix = Matrix4x4.TRS(
                    levelModuleWalls.transform.position,
                    levelModuleWalls.transform.rotation,
                    levelModuleWalls.transform.lossyScale
                    );

                // Set draw material
                if (selectionMode == SelectionMode.FILL)
                {
                    selectionFillMaterial.SetPass(0);
                }
                else
                {
                    selectionEraseMaterial.SetPass(0);
                }

                // Draw the selection
                Graphics.DrawMeshNow(mesh, matrix);
            }

            Handles.color = handlesColorBackup;      // Restore handles color
        }

        public static Mesh GenerateSelectionMesh(Vector3 min, Vector3 max)
        {
            // Mesh data
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            //List<Color32>   colors      = new List<Color32>();
            List<int> triangles = new List<int>();

            /* Vertices
            *   B---C      1---2
            *  /|  /|     /|  /|
            * A---D |    0---3 |
            * | F-|-G    | 5-|-6
            * |/  |/     |/  |/
            * E---H      4---7
            * 
            * E = Min
            * C = Max
            */

            Vector3 vA = new Vector3(min.x, max.y, min.z);
            Vector3 vB = new Vector3(min.x, max.y, max.z);
            Vector3 vC = new Vector3(max.x, max.y, max.z);
            Vector3 vD = new Vector3(max.x, max.y, min.z);
            Vector3 vE = new Vector3(min.x, min.y, min.z);
            Vector3 vF = new Vector3(min.x, min.y, max.z);
            Vector3 vG = new Vector3(max.x, min.y, max.z);
            Vector3 vH = new Vector3(max.x, min.y, min.z);

            // BAEF
            #region BAEF
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vB);
                vertices.Add(vA);
                vertices.Add(vE);
                vertices.Add(vF);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // B-A   0-1
                // |\| = |\|
                // F-E   3-2
                // BAE
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // BEF
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion

            // DCGH
            #region DCGH
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vD);
                vertices.Add(vC);
                vertices.Add(vG);
                vertices.Add(vH);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // D-C   0-1
                // |\| = |\|
                // H-G   3-2

                // HDC
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // HCG
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion

            // HEFG
            #region HEFG
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vH);
                vertices.Add(vE);
                vertices.Add(vF);
                vertices.Add(vG);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // H-E   0-1
                // |\| = |\|
                // G-F   3-2

                // EFG
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // EGH
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion

            // BCDA
            #region BCDA
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vB);
                vertices.Add(vC);
                vertices.Add(vD);
                vertices.Add(vA);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // B-C   0-1
                // |\| = |\|
                // A-D   3-2

                // BCD
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // BDA
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion

            // ADHE
            #region ADHE
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vA);
                vertices.Add(vD);
                vertices.Add(vH);
                vertices.Add(vE);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // A-D   0-1
                // |\| = |\|
                // E-H   3-2

                // ADH
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // AHE
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion

            // CBFG
            #region CBFG
            {
                // Vertices
                int verticesCount = vertices.Count;
                vertices.Add(vC);
                vertices.Add(vB);
                vertices.Add(vF);
                vertices.Add(vG);

                // UV
                // TODO

                // NORMALS
                // TODO

                // TRIANGLES
                // C-B   0-1
                // |\| = |\|
                // G-F   3-2

                // CBF
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 1);
                triangles.Add(verticesCount + 2);
                // CFG
                triangles.Add(verticesCount + 0);
                triangles.Add(verticesCount + 2);
                triangles.Add(verticesCount + 3);
            }
            #endregion


            // Compile mesh data
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();

            return mesh;
        }

    }

    /// <summary>
    /// Extention class providing LevelModuleWalls related utility methods.
    /// </summary>
    internal static class LevelModuleWallsExtention
    {
        /// <summary>
        /// Regenerate the mesh of a targeted level-module and set it on it's mesh filter (rendering) and collider.
        /// </summary>
        /// <param name="levelModuleWalls"></param>
        internal static void RefreshMesh(this LevelModuleWalls levelModuleWalls)
        {
            levelModuleWalls.SetMesh(levelModuleWalls.GenerateMesh());
        }
    }

    /// <summary>
    /// Class providing mathematics related utility methods.
    /// </summary>
    internal static class MathUtility
    {
        public static float RaseterizeFloor(float value, float steps)
        {
            return Mathf.Floor(value * steps) / steps;
        }
        public static float RaseterizeCeil(float value, float steps)
        {
            return Mathf.Ceil(value * steps) / steps;
        }
        public static Vector3 RaseterizeFloor(Vector3 value, float steps)
        {
            return new Vector3(
                RaseterizeFloor(value.x, steps),
                RaseterizeFloor(value.y, steps),
                RaseterizeFloor(value.z, steps)
                );
        }
        public static Vector3 RaseterizeCeil(Vector3 value, float steps)
        {
            return new Vector3(
                RaseterizeCeil(value.x, steps),
                RaseterizeCeil(value.y, steps),
                RaseterizeCeil(value.z, steps)
                );
        }
    }

}