using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gameplay
{
    [CustomEditor(typeof(AttachNode))]
    public class AttachNodeEditor : Editor
    {
        AttachNode t
        {
            get { return target as AttachNode; }
        }

        void OnSceneGUI()
        {
            DrawCenter();
            DrawRange();
        }

        private void DrawCenter()
        {
            Color color;
            Vector3 position;
            Quaternion rotation;
            float size;
            if (t.IsAttached())
            {
                color = Color.Lerp(Color.cyan, Color.white, Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * 2 * 2) * 0.5f + 0.5f);
                color.a = 0.25f;
                size = Mathf.Lerp(0f, t.range, Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * 2) * 0.5f + 0.5f);
            }
            else
            {
                color = Color.gray;
                size = 0.25f;
            }
            Handles.color = color;

            position = t.transform.position;
            rotation = Quaternion.identity;

            Handles.SphereCap(0, position, rotation, size);
        }

        private void DrawRange()
        {
            Color color;
            Vector3 position;
            Quaternion rotation;
            if (t.IsAttached())
            {
                color = Color.Lerp(Color.cyan, Color.white, Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * 2 * 2) * 0.5f + 0.5f);
            }
            else
            {
                color = Color.gray;
            }
            Handles.color = color;

            position = t.transform.position;
            rotation = Quaternion.Euler(0,0,0);
            
            Handles.DrawWireDisc(position, Vector3.right, t.range);
            Handles.DrawWireDisc(position, Vector3.up, t.range);
            Handles.DrawWireDisc(position, Vector3.forward, t.range);

            rotation = Quaternion.identity;
        }

    }
}