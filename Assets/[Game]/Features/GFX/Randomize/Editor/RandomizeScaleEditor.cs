using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RandomizeScale))]
[CanEditMultipleObjects]
public class RandomizeScaleEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Randomize Scale"))
        {
            serializedObject.Update();
            RandomizeScale t = target as RandomizeScale;
            Vector3 randomScale =
                new Vector3(
                    Random.Range(t.baseScale.x - t.randomRange.x / 2f, t.baseScale.x + t.randomRange.x / 2f),
                    Random.Range(t.baseScale.y - t.randomRange.y / 2f, t.baseScale.y + t.randomRange.y / 2f),
                    Random.Range(t.baseScale.z - t.randomRange.z / 2f, t.baseScale.z + t.randomRange.z / 2f)
                    );
            t.transform.localScale = randomScale;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
