using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LevelModule {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class LevelModuleWalls : MonoBehaviour {

        // DEBUG - Configuration
        [Header("DEBUG - Configuration")]

        [HideInInspector]
        [SerializeField]
        public Color colorDebug;

        [SerializeField]
        public bool[] _voxelMap; // Voxel 3d map

        public bool needRefresh = true;

        // Constants
        public const int Resolution = 16;

        public void InitialiseIfNeeded() {
            if (_voxelMap == null) {
                Make();
                Draw();
            }
        }

        public void Make() {
            // Create map
            _voxelMap = new bool[Resolution * Resolution * Resolution];
            // Fill
            for (int x = 0; x < Resolution; ++x) {
                for (int y = 0; y < Resolution; ++y) {
                    for (int z = 0; z < Resolution; ++z) {
                        // Calaculate unidimentional index
                        int index = x + y * Resolution + z * Resolution * Resolution;
                        //index = x << 0 + y << Resolution + z << Resolution * Resolution;
                        // Fill
                        _voxelMap[index] = true;
                    }
                }
            }

            //// Pierce hole
            //const int wallWidth = 1;
            //for (int x = wallWidth; x < Resolution - wallWidth; ++x) {
            //    for (int y = wallWidth; y < Resolution - wallWidth; ++y) {
            //        for (int z = wallWidth; z < Resolution - wallWidth; ++z) {
            //            _voxelMap[x, y, z] = false;
            //        }
            //    }
            //}
        }

        public void Draw() {
            // Mesh
            Mesh mesh = WallsHelper.GenerateMesh(_voxelMap);
            mesh.name = string.Format("Level Module Walls [{0}]", gameObject.GetHashCode());
            mesh.RecalculateBounds();
            // Filter
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.mesh = mesh;
            // Collider
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        //public void Start() {
        //    Make();
        //    Draw();
        //}

        public void Add(Vector3 min, Vector3 max) {
            for (
                int x = Mathf.Max(0, Mathf.FloorToInt(min.x));
                x < Mathf.Min(Mathf.FloorToInt(max.x), Resolution);
                ++x
            ) {

                for (
                    int y = Mathf.Max(0, Mathf.FloorToInt(min.y));
                    y < Mathf.Min(Mathf.FloorToInt(max.y), Resolution);
                    ++y
                ) {
                    for (
                        int z = Mathf.Max(0, Mathf.FloorToInt(min.z));
                        z < Mathf.Min(Mathf.FloorToInt(max.z), Resolution);
                        ++z
                    ) {
                        // Calaculate unidimentional index
                        int index = x + y * Resolution + z * Resolution * Resolution;
                        // Fill
                        _voxelMap[index] = true;
                    }
                }
            }
        }
        public void Remove(Vector3 min, Vector3 max) {
            for (
                int x = Mathf.Max(0, Mathf.FloorToInt(min.x));
                x < Mathf.Min(Mathf.FloorToInt(max.x), Resolution);
                ++x
            ) {

                for (
                    int y = Mathf.Max(0, Mathf.FloorToInt(min.y));
                    y < Mathf.Min(Mathf.FloorToInt(max.y), Resolution);
                    ++y
                ) {
                    for (
                        int z = Mathf.Max(0, Mathf.FloorToInt(min.z));
                        z < Mathf.Min(Mathf.FloorToInt(max.z), Resolution);
                        ++z
                    ) {
                        // Calaculate unidimentional index
                        int index = x + y * Resolution + z * Resolution * Resolution;
                        // Erase
                        _voxelMap[index] = false;
                    }
                }
            }
        }

        public void Start() {
            Draw();

            Debug.Log(colorDebug);
        }

    }

    public class WallsHelper {

        public static Mesh GenerateMesh(bool[] voxelMap) {

            // Map data
            int             resolution  = LevelModuleWalls.Resolution;

            // Mesh data
            List<Vector3>   vertices    = new List<Vector3>();
            List<Vector2>   uv          = new List<Vector2>();
            List<Vector3>   normals     = new List<Vector3>();
          //List<Color32>   colors      = new List<Color32>();
            List<int>       triangles   = new List<int>();
            
            for (int x = 0; x < resolution; ++x) {
                for (int y = 0; y < resolution; ++y) {
                    for (int z = 0; z < resolution; ++z) {
                        // Calaculate unidimentional index
                        int index = x + y * resolution + z * resolution * resolution;

                        // Check if voxel is empty
                        if (!voxelMap[index]) {
                            continue;
                        }
                        // Draw voxel faces

                        // Check x-
                        int indexNegX = (x - 1) + y * resolution + z * resolution * resolution;
                        #region x- check
                        if (
                            x == 0 ||               // Face is on an extremity
                            !voxelMap[indexNegX]      // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
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
                        int indexPosX = (x + 1) + y * resolution + z * resolution * resolution;
                        #region x+ check
                        if (
                            x == resolution - 1 ||  // Face is on an extremity
                            !voxelMap[indexPosX]      // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
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
                        int indexNegY = x + (y - 1) * resolution + z * resolution * resolution;
                        #region y- check
                        if (
                            y == 0 ||                   // Face is on an extremity
                            !voxelMap[indexNegY]      // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
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
                        int indexPosY = x + (y + 1) * resolution + z * resolution * resolution;
                        #region y+ check
                        if (
                            y == resolution - 1 ||      // Face is on an extremity
                            !voxelMap[indexPosY]      // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
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
                        int indexNegZ = x + y * resolution + (z - 1) * resolution * resolution;
                        #region z- check
                        if (
                            z == 0 ||                   // Face is on an extremity
                            !voxelMap[indexNegZ]     // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 0) / (float)resolution
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
                        int indexPosZ = x + y * resolution + (z + 1) * resolution * resolution;
                        #region z+ check
                        if (
                            z == resolution - 1 ||      // Face is on an extremity
                            !voxelMap[indexPosZ]      // Face is facing an empty voxel
                        ) {
                            // The face needs to be drawn

                            /** Compute data **/
                            // vertices
                            // A-B   0-1
                            // |\| = |\|
                            // D-C   3-2
                            Vector3 vA = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vB = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 1) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vC = new Vector3(
                                (float)(x + 0) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
                                );
                            Vector3 vD = new Vector3(
                                (float)(x + 1) / (float)resolution,
                                (float)(y + 0) / (float)resolution,
                                (float)(z + 1) / (float)resolution
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
            for (int i = 0; i < vertices.Count; ++i) {
                vertices[i] = vertices[i] + translation;
            }

            // Compile mesh data
            Mesh mesh = new Mesh();
            mesh.vertices   = vertices.ToArray();
            mesh.uv         = uv.ToArray();
            mesh.normals    = normals.ToArray();
            mesh.triangles  = triangles.ToArray();
            
            return mesh;
        }

    }

}