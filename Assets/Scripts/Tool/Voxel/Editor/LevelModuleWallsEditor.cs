using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelModule {

    [CustomEditor(typeof(LevelModuleWalls))]
    [CanEditMultipleObjects]
    public class LevelModuleWallsEditor : Editor {

        public enum SelectionMode {
            NONE,
            ADD,
            REMOVE
        }

        Vector3? startPos = null;
        Vector3? endPos = null;
        SelectionMode selectionArrayMode = SelectionMode.NONE;

        public void OnEnable() {
            LevelModuleWalls levelModuleWalls = (LevelModuleWalls)target;
            levelModuleWalls.InitialiseIfNeeded();

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void OnUndoRedo() {
            LevelModuleWalls levelModuleWalls = (LevelModuleWalls)target;
            if(levelModuleWalls != null) {
                levelModuleWalls.Draw();
            }
        }

        public override void OnInspectorGUI() {
            LevelModuleWalls levelModuleWalls = (LevelModuleWalls)target;
            DrawDefaultInspector();
            GUILayout.Label("Level Module Walls");
            if (GUILayout.Button("Reset")) {
                levelModuleWalls.Make();
                levelModuleWalls.Draw();
            }
        }
        
        private void OnSceneGUI() {
            // Check if the user has his mouse over a scene window
            EditorWindow sceneEditorWindow = SceneView.mouseOverWindow;
            if (sceneEditorWindow == null) return;

            LevelModuleWalls levelModuleWalls = (LevelModuleWalls)target;

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
            ) {

                // DEBUG - Draw a sphere at the hit point found position
                Handles.SphereCap(0, hit.point, Quaternion.identity, 0.01f);

                // Check for selection array start
                if (!startPos.HasValue) {

                    // Start point isn't defined

                    // Get local hit point
                    Vector3 localHitPoint = ColliderHelper.FromWorldToLocalPosition(hit.collider.transform, hit.point);

                    // Check for left click down
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        Event.current.Use();
                        // Set selction array start point
                        startPos = hit.point;
                        startPos += hit.normal * (0.25f / LevelModuleWalls.Resolution);
                        // Set selection array as an add array
                        selectionArrayMode = SelectionMode.ADD;
                    } else
                    // Check for right click down
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
                        Event.current.Use();
                        // Set selction array start point
                        startPos = hit.point;
                        startPos -= hit.normal * (0.25f / LevelModuleWalls.Resolution);
                        // Set selection array as a remove array
                        selectionArrayMode = SelectionMode.REMOVE;
                    }
                } else {

                    // Start point is defined

                    // Refresh end pos
                    endPos = hit.point;
                    switch (selectionArrayMode) {
                        case SelectionMode.ADD:
                            endPos += hit.normal * (0.25f / LevelModuleWalls.Resolution);
                            break;
                        case SelectionMode.REMOVE:
                            endPos -= hit.normal * (0.25f / LevelModuleWalls.Resolution);
                            break;
                    }
                }
            }

            // Check if a selection is active
            if (selectionArrayMode != SelectionMode.NONE && startPos.HasValue && endPos.HasValue) {
                // Destroy selection if the user release the input associated 
                
                    // Get selection's min/max
                    Vector3 localStartPoint = ColliderHelper.FromWorldToLocalPosition(levelModuleWalls.transform, startPos.Value);
                Vector3 localEndPoint = ColliderHelper.FromWorldToLocalPosition(levelModuleWalls.transform, endPos.Value);
                Vector3 min = new Vector3(
                    Mathf.Min(localStartPoint.x, localEndPoint.x),
                    Mathf.Min(localStartPoint.y, localEndPoint.y),
                    Mathf.Min(localStartPoint.z, localEndPoint.z)
                    );
                min = RaseterizeFloor(min, steps: LevelModuleWalls.Resolution);
                Vector3 max = new Vector3(
                    Mathf.Max(localStartPoint.x, localEndPoint.x),
                    Mathf.Max(localStartPoint.y, localEndPoint.y),
                    Mathf.Max(localStartPoint.z, localEndPoint.z)
                    );
                max = RaseterizeCeil(max, steps: LevelModuleWalls.Resolution);

                min += Vector3.one * 0.5f;
                max += Vector3.one * 0.5f;
                min *= LevelModuleWalls.Resolution;
                max *= LevelModuleWalls.Resolution;

                // Check for left click up
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && selectionArrayMode == SelectionMode.ADD) {
                    Event.current.Use();

                    // Add voxels
                    Undo.RecordObject(levelModuleWalls, string.Format("Add voxels from {0}", levelModuleWalls.name));
                    levelModuleWalls.Add(min, max);
                    levelModuleWalls.Draw();

                    // DEBUG
                    levelModuleWalls.colorDebug = Color.green;

                    // Remove selction array start and end points
                    startPos = null;
                    endPos = null;
                    // Set selection array as non existent
                    selectionArrayMode = SelectionMode.NONE;
                } else
                // Check for left click down
                if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && selectionArrayMode == SelectionMode.REMOVE) {
                    Event.current.Use();

                    // Remove voxels
                    Undo.RecordObject(levelModuleWalls, string.Format("Removed voxels from {0}", levelModuleWalls.name));
                    levelModuleWalls.Remove(min, max);
                    levelModuleWalls.Draw();

                    // DEBUG
                    levelModuleWalls.colorDebug = Color.red;

                    // Remove selction array start and end points
                    startPos = null;
                    endPos = null;
                    // Set selection array as non existent
                    selectionArrayMode = SelectionMode.NONE;
                }
            }

            // DEBUG - Two points
            Color handlesColorBackup = Handles.color; // Backup handles color
            if (startPos.HasValue) {
                // DEBUG - Draw a sphere at the start point position
                Handles.color = Color.red;
                Handles.SphereCap(0, startPos.Value, Quaternion.identity, 0.05f);
            }
            if (endPos.HasValue) {
                // DEBUG - Draw a sphere at the start point position
                Handles.color = Color.blue;
                Handles.SphereCap(0, endPos.Value, Quaternion.identity, 0.05f);
            }

            if (startPos.HasValue && endPos.HasValue) {
                Vector3 middlePoint = Vector3.Lerp(startPos.Value, endPos.Value, 0.5f);
                Handles.color = new Color32(0, 255, 255, 128);

                // Get local start point
                Vector3 localStartPoint = ColliderHelper.FromWorldToLocalPosition(levelModuleWalls.transform, startPos.Value);
                Vector3 localEndPoint = ColliderHelper.FromWorldToLocalPosition(levelModuleWalls.transform, endPos.Value);

                Vector3 min = new Vector3(
                    Mathf.Min(localStartPoint.x, localEndPoint.x),
                    Mathf.Min(localStartPoint.y, localEndPoint.y),
                    Mathf.Min(localStartPoint.z, localEndPoint.z)
                    );
                min = RaseterizeFloor(min, steps: LevelModuleWalls.Resolution);
                Vector3 max = new Vector3(
                    Mathf.Max(localStartPoint.x, localEndPoint.x),
                    Mathf.Max(localStartPoint.y, localEndPoint.y),
                    Mathf.Max(localStartPoint.z, localEndPoint.z)
                    );
                max = RaseterizeCeil(max, steps: LevelModuleWalls.Resolution);

                Mesh mesh = GenerateMesh(min, max); 
                Matrix4x4 matrix = Matrix4x4.TRS(
                    levelModuleWalls.transform.position,
                    levelModuleWalls.transform.rotation,
                    levelModuleWalls.transform.lossyScale * 20
                    );
                Graphics.DrawMeshNow(mesh, matrix);
            }

            Handles.color = handlesColorBackup;      // Restore handles color

        }

        public static Mesh GenerateMesh(Vector3 min, Vector3 max) {
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

        public static float RaseterizeFloor(float value, float steps) {
            return Mathf.Floor(value * steps) / steps;
        }
        public static float RaseterizeCeil(float value, float steps) {
            return Mathf.Ceil(value * steps) / steps;
        }
        public static Vector3 RaseterizeFloor(Vector3 value, float steps) {
            return new Vector3(
                RaseterizeFloor(value.x, steps),
                RaseterizeFloor(value.y, steps),
                RaseterizeFloor(value.z, steps)
                );
        }
        public static Vector3 RaseterizeCeil(Vector3 value, float steps) {
            return new Vector3(
                RaseterizeCeil(value.x, steps),
                RaseterizeCeil(value.y, steps),
                RaseterizeCeil(value.z, steps)
                );
        }

    }

}