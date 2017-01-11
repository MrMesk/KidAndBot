using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class YuME_freezeMap : EditorWindow
{
    static GameObject frozenMap;

    public static void combineTiles()
    {
        if (YuME_mapEditor.tileMapParent)
        {
            frozenMap = new GameObject();
            frozenMap.transform.parent = YuME_mapEditor.tileMapParent.transform;
            frozenMap.name = "frozenMap";

            List<GameObject> tilesToCombine = new List<GameObject>();

            EditorUtility.DisplayProgressBar("Building Frozen Map", "Finding Tiles to Freeze", 0f);

            foreach (Transform layer in YuME_mapEditor.tileMapParent.transform)
            {
                foreach (Transform tile in layer)
                {
                    foreach (Transform mesh in tile)
                    {
                        tilesToCombine.Add(mesh.gameObject);
                    }
                }
            }

            tilesToCombine = tilesToCombine.OrderBy(x => x.GetComponent<MeshRenderer>().sharedMaterial.name).ToList();

            Material previousMaterial = tilesToCombine[0].GetComponent<MeshRenderer>().sharedMaterial;

            List<CombineInstance> combine = new List<CombineInstance>();
            CombineInstance tempCombine = new CombineInstance();
            int vertexCount = 0;

            foreach (GameObject mesh in tilesToCombine)
            {
                vertexCount += mesh.GetComponent<MeshFilter>().sharedMesh.vertexCount;

                if (vertexCount > 60000)
                {
                    vertexCount = 0;
                    newSubMesh(combine, mesh.GetComponent<MeshRenderer>().sharedMaterial);
                    combine = new List<CombineInstance>();
                }

                if (mesh.GetComponent<MeshRenderer>().sharedMaterial.name != previousMaterial.name)
                {
                    newSubMesh(combine, mesh.GetComponent<MeshRenderer>().sharedMaterial);
                    combine = new List<CombineInstance>();
                }

                tempCombine.mesh = mesh.GetComponent<MeshFilter>().sharedMesh;
                tempCombine.transform = mesh.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                combine.Add(tempCombine);
                previousMaterial = mesh.GetComponent<MeshRenderer>().sharedMaterial;
            }

            newSubMesh(combine, previousMaterial);

            foreach (Transform layer in YuME_mapEditor.tileMapParent.transform)
            {
                if (layer.name.Contains("layer"))
                {
                    layer.gameObject.SetActive(false);
                }
            }

            YuME_brushFunctions.cleanSceneOfBrushObjects();

            EditorUtility.ClearProgressBar();
        }
    }

    static void newSubMesh(List<CombineInstance> combine, Material mat)
    {
        GameObject subMesh = new GameObject();
        subMesh.transform.parent = frozenMap.transform;
        subMesh.name = "subMesh";

        MeshFilter subMeshFilter = subMesh.AddComponent<MeshFilter>();
        subMeshFilter.sharedMesh = new Mesh();
        subMeshFilter.sharedMesh.name = "subMesh";
        subMeshFilter.sharedMesh.CombineMeshes(combine.ToArray());
        MeshRenderer meshRenderer = subMesh.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;
        subMesh.AddComponent<MeshCollider>().sharedMesh = subMeshFilter.sharedMesh;

        subMesh.SetActive(true);

    }
}