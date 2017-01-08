using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gameplay
{
    [CustomEditor(typeof(CharacterCamera))]
    public class CharacterCameraEditor : Editor
    {

        CharacterCamera t {
            get { return target as CharacterCamera; }
        }


        /***********
         * HANDLES *
         ***********/

        private class HandleData
        {
            public delegate Vector3 GetPositionHandler();
            public delegate Quaternion GetRotationHandler();
            public GetPositionHandler getPosition;
            public GetRotationHandler getRotation;
            public Color color = Color.black;

            public HandleData(GetPositionHandler getPosition, GetRotationHandler getRotation, Color color)
            {
                this.getPosition = getPosition;
                this.getRotation = getRotation;
                this.color = color;
            }
        }

        // Current horizontal position around character
        HandleData currentHorizontalPositionAroundCharacter;
        // Current vertical position around character
        HandleData currentVerticalPositionAroundCharacter;
        // Current target position around character
        HandleData currentTargetPositionAroundCharacter;

        public void InitHandles()
        {
            // Current horizontal position around character
            currentHorizontalPositionAroundCharacter =
            new HandleData(
                delegate
                {
                    return t.GetLocalHorizontalPositionAroundCharacter(t.transform.position - t.kid.transform.position);
                },
                delegate
                {
                    return Quaternion.identity;
                },
                Color.green
                );

            // Current vertical position around character
            currentVerticalPositionAroundCharacter =
            new HandleData(
                delegate
                {
                    return t.GetLocalVerticalPositionAroundCharacter(t.transform.position - t.kid.transform.position);
                },
                delegate
                {
                    return Quaternion.identity;
                },
                Color.red
                );

            // Current target position around character
            currentTargetPositionAroundCharacter =
            new HandleData(
                delegate
                {
                    return
                        t.GetLocalHorizontalPositionAroundCharacter(t.directionalTargetDirection) +
                        t.GetLocalVerticalPositionAroundCharacter(t.transform.position - t.kid.transform.position);
                },
                delegate
                {
                    return Quaternion.identity;
                },
                Color.yellow
                );

        }

        private void OnEnable()
        {
            // Initialise handles
            InitHandles();
        }
        
        private void DisplayCameraPreview(Camera camera)
        {
            RenderTexture rtBackup = camera.targetTexture;
            int height = 128;
            int width = (camera.pixelWidth * height) / camera.pixelHeight;
            RenderTexture cameraPreview = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 16);
            camera.targetTexture = cameraPreview;
            camera.Render();
            camera.targetTexture = rtBackup;
            //Handles.BeginGUI();

            GUILayout.Window(0, new Rect(Screen.width - width - 32, Screen.height - height - 64, width, height), (id) => {
                // Content of window here
                Rect previewRect = GUILayoutUtility.GetRect(width, height);
                GUI.DrawTexture(previewRect, cameraPreview);
                DestroyImmediate(cameraPreview, true); // Avoid memory leak
            }, "Camera Preview");
            
            //Handles.EndGUI();
        }

        void OnSceneGUI()
        {
            if(t == null)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                t.LookAtTarget(t.GetTargetPosition());
            }
            DisplayCameraPreview(t.unityCamera);

            // Draw handles [Setup]
            Vector3 handlePosition;
            Quaternion handleRotation;
            Color handleColor;


            // Horizontal : Around
            Vector3 currentHorizontalPositionAroundCharacter = this.currentHorizontalPositionAroundCharacter.getPosition(); // t.GetHorizontalPositionAroundCharacter(t.transform.position);
            Handles.color = this.currentHorizontalPositionAroundCharacter.color; // Color.green;
            Handles.DrawWireDisc(t.kid.transform.position, Vector3.up, t.distanceAround); // Disc
            Handles.SphereCap(
                0,
                t.kid.transform.position + currentHorizontalPositionAroundCharacter,
                Quaternion.identity,
                0.5f
                );

            // Vertical : Height
            Vector3 currentVerticalPositionAroundCharacter = this.currentVerticalPositionAroundCharacter.getPosition(); // t.GetVerticalPosition(t.transform.position);
            Handles.color = this.currentVerticalPositionAroundCharacter.color; // Color.red;
            Handles.DrawLine(
                 t.kid.transform.position + currentHorizontalPositionAroundCharacter,
                 t.kid.transform.position + currentHorizontalPositionAroundCharacter + currentVerticalPositionAroundCharacter
                );
            Handles.SphereCap(
                0,
                 t.kid.transform.position + currentHorizontalPositionAroundCharacter + currentVerticalPositionAroundCharacter,
                Quaternion.identity,
                0.5f
                );
            Handles.DrawLine(
                 Vector3.zero,
                 currentVerticalPositionAroundCharacter
                );


            Color lineColor;
            Vector3 arrowPosition;
            Quaternion arrowRotation;
            // CORRECTION

            // Line
            lineColor = Color.cyan;
            lineColor.a = 0.25f;
            Handles.color = lineColor;
            Handles.DrawLine(
                 t.kid.transform.position + currentHorizontalPositionAroundCharacter + currentVerticalPositionAroundCharacter,
                 t.transform.position
                );

            // Cone
            Handles.color = Color.cyan;
            DrawAnimatedTargetLine(
                t.transform.position,
                t.kid.transform.position + currentHorizontalPositionAroundCharacter + currentVerticalPositionAroundCharacter,
                1f,
                Color.cyan,
                4f
                );

            // TARGET

            // Line
            lineColor = Color.magenta;
            lineColor.a = 0.25f;
            Handles.color = lineColor;
            Handles.DrawLine(
                t.transform.position,
                t.GetTargetPosition()
                );

            // Cone
            Handles.color = Color.magenta;
            arrowPosition = Vector3.MoveTowards(t.transform.position, t.GetTargetPosition(), 1f);
            arrowRotation = Quaternion.LookRotation(arrowPosition - t.transform.position, Vector3.up);
            Handles.ConeCap(
                0,
                arrowPosition,
                arrowRotation,
                0.5f
                );


            // TARGET 2.0
            Vector3 currentTargetPositionAroundCharacter = this.currentTargetPositionAroundCharacter.getPosition(); // t.GetHorizontalPositionAroundCharacter(t.transform.position);
            Handles.color = this.currentTargetPositionAroundCharacter.color; // Color.green;
            Handles.SphereCap(
                0,
                t.kid.transform.position + currentTargetPositionAroundCharacter,
                Quaternion.identity,
                0.5f
                );
        }

        public void DrawAnimatedTargetLine(Vector3 start, Vector3 end, float stepLengh, Color color, float animationSpeed)
        {
            Vector3 spatialProgress = start;
            Vector3 handlePosition = spatialProgress;
            Quaternion handleRotation;
            Handles.color = color;
            float repeatedTime = 0;
            if (Vector3.Distance(spatialProgress, end) > stepLengh)
            {
                handleRotation = Quaternion.LookRotation(end - start);
            }
            else
            {
                handleRotation = Quaternion.identity;
            }
            while (Vector3.Distance(spatialProgress, end) > stepLengh)
            {
                repeatedTime = Mathf.Repeat(Time.realtimeSinceStartup * animationSpeed, stepLengh);
                handlePosition = Vector3.MoveTowards(spatialProgress, end, repeatedTime);
                spatialProgress = Vector3.MoveTowards(spatialProgress, end, stepLengh);
                Handles.ConeCap(
                    0,
                    handlePosition,
                    handleRotation,
                    0.25f
                    );
            }

        }
    }

}