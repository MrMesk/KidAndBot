using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LevelModule
{

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class LevelModuleWalls : MonoBehaviour
    {

        /*********
         * DEBUG *
         * *******/

        [HideInInspector]
        [SerializeField]
        public Color colorDebug;




        /********
         * DATA *
         * ******/

        // Serialized data

        /// <summary>
        /// The bit three dimentional array used to store the voxel 3D map in memory. This map is serialized and saved onto this gameobject.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        public bool[] _voxelMap;


        // Constants

        /// <summary>
        /// Constant defining in how much voxels a single level-module is subdivided.
        /// </summary>
        public const int BlockSubdivisions = 16;




        /*********
         * UNITY *
         * *******/
        
        // Editor

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// </summary>
        private void OnValidate()
        {
            // Initialise the voxel 3D map of this level-module
            InitialiseIfNeeded();
            // Generate this level-module's mesh and set it on both the filter (rendering) and collider.
            SetMesh(GenerateMesh());
        }

        // Runtime

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
        /// </summary>
        public void Start()
        {
            GenerateMesh();

            Debug.Log(colorDebug);
        }


        /// <summary>
        /// Initialise the voxel 3D map if it wasn't initialised or deserialised before.
        /// </summary>
        public void InitialiseIfNeeded()
        {
            if (_voxelMap == null)
            {
                Init();
            }
        }

        /*********
         * LOGIC *
         * *******/

        // Initialisation

        /// <summary>
        /// Initialise the voxel 3D map.
        /// </summary>
        public void Init()
        {
            // Create the map
            _voxelMap = new bool[BlockSubdivisions * BlockSubdivisions * BlockSubdivisions];

            // Fill it
            for (int x = 0; x < BlockSubdivisions; ++x)
            {
                for (int y = 0; y < BlockSubdivisions; ++y)
                {
                    for (int z = 0; z < BlockSubdivisions; ++z)
                    {
                        // Calaculate unidimentional index from 
                        int index = GetUnidimentionalIndex(x, y, z);
                        // Fill
                        _voxelMap[index] = true;
                    }
                }
            }

            //// Make it hollow
            //const int wallWidth = 1;
            //for (int x = wallWidth; x < BlockSubdivisions - wallWidth; ++x)
            //{
            //    for (int y = wallWidth; y < BlockSubdivisions - wallWidth; ++y)
            //    {
            //        for (int z = wallWidth; z < BlockSubdivisions - wallWidth; ++z)
            //        {
            //            // Calaculate unidimentional index from 
            //            int index = GetUnidimentionalIndex(x, y, z);
            //            // Fill
            //            _voxelMap[index] = true;
            //        }
            //    }
            //}
        }




        // Mesh

        /// <summary>
        /// Generate this level-module's mesh from the voxel 3D map.
        /// </summary>
        public Mesh GenerateMesh()
        {
            // Mesh
            Mesh mesh = WallsUtility.GenerateMesh(_voxelMap);
            mesh.name = string.Format("Level Module Walls [{0}]", gameObject.GetHashCode());
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Set a mesh on both this object mesh filter (rendering) and collider.
        /// </summary>
        /// <param name="mesh">The mesh to be set.</param>
        public void SetMesh(Mesh mesh)
        {
            // Filter
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.mesh = mesh;
            // Collider
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }




        // Edition

        #region Edition
#if UNITY_EDITOR
        /// <summary>
        /// Edit the voxel map by drawing voxels in the defined array.
        /// </summary>
        /// <param name="min">The minimum position of the rectangular array in local space.</param>
        /// <param name="max">The maximum position of the rectangular array in local space.</param>
        public void Edit_FillArray(Vector3 min, Vector3 max)
        {
            for (
                int x = Mathf.Max(0, Mathf.FloorToInt(min.x));
                x < Mathf.Min(Mathf.FloorToInt(max.x), BlockSubdivisions);
                ++x
            )
            {

                for (
                    int y = Mathf.Max(0, Mathf.FloorToInt(min.y));
                    y < Mathf.Min(Mathf.FloorToInt(max.y), BlockSubdivisions);
                    ++y
                )
                {
                    for (
                        int z = Mathf.Max(0, Mathf.FloorToInt(min.z));
                        z < Mathf.Min(Mathf.FloorToInt(max.z), BlockSubdivisions);
                        ++z
                    )
                    {
                        // Calaculate unidimentional index
                        int index = GetUnidimentionalIndex(x, y, z);
                        // Fill
                        _voxelMap[index] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Edit the voxel map by erasing voxels in the defined array.
        /// </summary>
        /// <param name="min">The minimum position of the rectangular array in local space.</param>
        /// <param name="max">The maximum position of the rectangular array in local space.</param>
        public void Edit_EraseArray(Vector3 min, Vector3 max)
        {
            for (
                int x = Mathf.Max(0, Mathf.FloorToInt(min.x));
                x < Mathf.Min(Mathf.FloorToInt(max.x), BlockSubdivisions);
                ++x
            )
            {

                for (
                    int y = Mathf.Max(0, Mathf.FloorToInt(min.y));
                    y < Mathf.Min(Mathf.FloorToInt(max.y), BlockSubdivisions);
                    ++y
                )
                {
                    for (
                        int z = Mathf.Max(0, Mathf.FloorToInt(min.z));
                        z < Mathf.Min(Mathf.FloorToInt(max.z), BlockSubdivisions);
                        ++z
                    )
                    {
                        // Calaculate unidimentional index
                        int index = GetUnidimentionalIndex(x, y, z);
                        // Erase
                        _voxelMap[index] = false;
                    }
                }
            }
        }
#endif
        #endregion Edition




        // Get / Set

        /// <summary>
        /// Calculate the unidimentional index of a voxel from it's three dimentional indexes.
        /// </summary>
        public int GetUnidimentionalIndex(int x, int y, int z)
        {
            return x + y * BlockSubdivisions + z * BlockSubdivisions * BlockSubdivisions;
        }

    }

    internal class WallsUtility
    {

        public static Mesh GenerateMesh(bool[] voxelMap)
        {

            // Map data
            int subdivisions = LevelModuleWalls.BlockSubdivisions;

            // Mesh data
            List<Vector3> vertices  = new List<Vector3>();
            List<Vector2> uv        = new List<Vector2>();
            List<Vector3> normals   = new List<Vector3>();
            //List<Color32> colors  = new List<Color32>();
            List<int> triangles     = new List<int>();

            for (int x = 0; x < subdivisions; ++x)
            {
                for (int y = 0; y < subdivisions; ++y)
                {
                    for (int z = 0; z < subdivisions; ++z)
                    {
                        // Calaculate unidimentional index
                        int index = x + y * subdivisions + z * subdivisions * subdivisions;

                        // Check if voxel is empty
                        if (!voxelMap[index])
                        {
                            continue;
                        }
                        // Draw voxel faces

                        // Check x-
                        int indexNegX = (x - 1) + y * subdivisions + z * subdivisions * subdivisions;
                        #region x- check
                        if (
                            x == 0 ||               // Face is on an extremity
                            !voxelMap[indexNegX]      // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = Vector3.left;
                            Vector3 nB = Vector3.left;
                            Vector3 nC = Vector3.left;
                            Vector3 nD = Vector3.left;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, B, C
                            int[] t2 = { 0, 2, 3 }; // A, C, D

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(uvB);
                            uv.Add(uvC);
                            uv.Add(uvD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                        // Check x+
                        int indexPosX = (x + 1) + y * subdivisions + z * subdivisions * subdivisions;
                        #region x+ check
                        if (
                            x == subdivisions - 1 ||  // Face is on an extremity
                            !voxelMap[indexPosX]      // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = Vector3.right;
                            Vector3 nB = Vector3.right;
                            Vector3 nC = Vector3.right;
                            Vector3 nD = Vector3.right;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, C, B
                            int[] t2 = { 0, 2, 3 }; // A, D, C

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(vB);
                            uv.Add(vC);
                            uv.Add(vD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                        // Check y-
                        int indexNegY = x + (y - 1) * subdivisions + z * subdivisions * subdivisions;
                        #region y- check
                        if (
                            y == 0 ||                   // Face is on an extremity
                            !voxelMap[indexNegY]      // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = Vector3.down;
                            Vector3 nB = Vector3.down;
                            Vector3 nC = Vector3.down;
                            Vector3 nD = Vector3.down;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, C, B
                            int[] t2 = { 0, 2, 3 }; // A, D, C

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(vB);
                            uv.Add(vC);
                            uv.Add(vD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                        // Check y+
                        int indexPosY = x + (y + 1) * subdivisions + z * subdivisions * subdivisions;
                        #region y+ check
                        if (
                            y == subdivisions - 1 ||      // Face is on an extremity
                            !voxelMap[indexPosY]      // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = Vector3.up;
                            Vector3 nB = Vector3.up;
                            Vector3 nC = Vector3.up;
                            Vector3 nD = Vector3.up;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, C, B
                            int[] t2 = { 0, 2, 3 }; // A, D, C

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(vB);
                            uv.Add(vC);
                            uv.Add(vD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                        // Check z-
                        int indexNegZ = x + y * subdivisions + (z - 1) * subdivisions * subdivisions;
                        #region z- check
                        if (
                            z == 0 ||                   // Face is on an extremity
                            !voxelMap[indexNegZ]     // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 0) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = -Vector3.forward;
                            Vector3 nB = -Vector3.forward;
                            Vector3 nC = -Vector3.forward;
                            Vector3 nD = -Vector3.forward;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, C, B
                            int[] t2 = { 0, 2, 3 }; // A, D, C

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(vB);
                            uv.Add(vC);
                            uv.Add(vD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                        // Check z+
                        int indexPosZ = x + y * subdivisions + (z + 1) * subdivisions * subdivisions;
                        #region z+ check
                        if (
                            z == subdivisions - 1 ||      // Face is on an extremity
                            !voxelMap[indexPosZ]      // Face is facing an empty voxel
                        )
                        {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 1) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 0) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)subdivisions,
                                (float)(y + 0) / (float)subdivisions,
                                (float)(z + 1) / (float)subdivisions
                                );

                            // uv
                            Vector2 uvA = Vector2.zero; // TODO
                            Vector2 uvB = Vector2.zero; // TODO
                            Vector2 uvC = Vector2.zero; // TODO
                            Vector2 uvD = Vector2.zero; // TODO

                            // normals
                            Vector3 nA = Vector3.forward;
                            Vector3 nB = Vector3.forward;
                            Vector3 nC = Vector3.forward;
                            Vector3 nD = Vector3.forward;

                            // triangles
                            int[] t1 = { 0, 1, 2 }; // A, C, B
                            int[] t2 = { 0, 2, 3 }; // A, D, C

                            /** Store data **/
                            // vertices
                            int currentVertexCount = vertices.Count;
                            vertices.Add(vA);
                            vertices.Add(vB);
                            vertices.Add(vC);
                            vertices.Add(vD);

                            // uv
                            uv.Add(uvA);
                            uv.Add(vB);
                            uv.Add(vC);
                            uv.Add(vD);

                            // normals
                            normals.Add(nA);
                            normals.Add(nB);
                            normals.Add(nC);
                            normals.Add(nD);

                            // triangles
                            triangles.Add(currentVertexCount + t1[0]);
                            triangles.Add(currentVertexCount + t1[1]);
                            triangles.Add(currentVertexCount + t1[2]);
                            triangles.Add(currentVertexCount + t2[0]);
                            triangles.Add(currentVertexCount + t2[1]);
                            triangles.Add(currentVertexCount + t2[2]);
                        }
                        #endregion

                    }
                }
            }

            // Center mesh
            Vector3 translation = Vector3.one * -0.5f;
            for (int i = 0; i < vertices.Count; ++i)
            {
                vertices[i] = vertices[i] + translation;
            }

            // Merge mesh data
            Mesh mesh       = new Mesh();
            mesh.vertices   = vertices.ToArray();
            mesh.uv         = uv.ToArray();
            mesh.normals    = normals.ToArray();
            mesh.triangles  = triangles.ToArray();

            return mesh;
        }

    }

}