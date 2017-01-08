using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Gameplay
{
    [ExecuteInEditMode]
    public class CharacterCamera : MonoBehaviour
    {
        /*********
         * UNITY *
         *********/
        private void Update()
        {
            if (kid.directional.velocity.magnitude > 0.1f)
            {
                directionalTargetDirection = kid.directional.velocity.normalized;
            }
        }

        private void OnRenderingPreCull(Camera camera = null)
        {
            // Rendering
            Rendering(Time.deltaTime);
        }

        private void OnValidate()
        {
            // _position around
            // Repeat x in [0;360[
            while (_around.x >= 360)
            {
                _around.x -= 360;
            }
            while (_around.x < 0)
            {
                _around.x += 360;
            }
            // Repeat y in [0;360[
            while (_around.y >= 360)
            {
                _around.y -= 360;
            }
            while (_around.y < 0)
            {
                _around.y += 360;
            }
        }

        // Container transform
        [System.NonSerialized]
        protected Camera _unityCamera = null;
        public Camera unityCamera
        {
            get
            {
                return _unityCamera ?? (_unityCamera = GetComponentInChildren<Camera>());
            }
        }

        [Header("Configuration")]
        public Transform primaryCharacterTransform;
        public Transform secondaryCharacterTransform;

        public KidCharacter kid;

        public void Awake()
        {
            // Bind camera
            //unityCamera.transform.position = Vector3.zero;
            _unityCamera = GetComponentInChildren<Camera>();
        }

        public void Start()
        {
            if (Application.isPlaying)
            {
                Camera.onPreCull += OnRenderingPreCull;
                kid.compass.eDrasticlyChangedNormal += delegate
                {
                    Debug.Log("Lowered");
                    targetPosition = GetTargetPosition();
                    TickPositionAroundCharacter(0, true);
                    LookAtTarget();
                };
            }

        }

        public void OnApplicationQuit()
        {
            Camera.onPreCull -= OnRenderingPreCull;
        }

        private void FixedUpdate()
        {
            LogicTick(Time.fixedDeltaTime);
        }

        /// <summary>
        /// Controller is called every frame. Use it for everything input related.
        /// </summary>
        protected virtual void ControllerTick(PlayerInput.Controller input, float dt) { }


        /// <summary>
        /// Logic tick is called more than once a frame. Use it for everything logic related.
        /// </summary>
        protected virtual void LogicTick(float dt)
        {

            //float angle = Vector3.Angle(directionalTargetDirection, kid.directional.velocity);
            //directionalTargetDirection =
            //    Vector3.RotateTowards(
            //        directionalTargetDirection,
            //        kid.directional.velocity,
            //        (angle * Mathf.Deg2Rad) * dt * 0.5f,
            //        0
            //        );

            // Tick directional
            // Tick directional direction slider
            if (directionalTargetDirection.magnitude > 0.1f)
            {
                // Tick directional direction slider
                directionalSlider = Mathf.MoveTowards(directionalSlider, 1, dt);
            }
            else
            {
                directionalSlider = Mathf.MoveTowards(directionalSlider, 0, dt);
            }

            //
            TickPositionAroundCharacter(dt);

        }


        /*********
         * STATE *
         *********/

        [SerializeField]
        private Vector2 _around;
        public Vector2 around
        {
            get
            {
                return _around;
            }

            set
            {
                _around = value;
                // Repeat x in [0;360[
                while (_around.x >= 360)
                {
                    _around.x -= 360;
                }
                while (_around.x < 0)
                {
                    _around.x += 360;
                }
                // Repeat y in [0;360[
                while (_around.y >= 360)
                {
                    _around.y -= 360;
                }
                while (_around.y < 0)
                {
                    _around.y += 360;
                }
            }

        }
        [System.NonSerialized]
        public float distanceAround = 10;

        public void OnPreRender()
        {
        }



        /*************
         * RENDERING *
         *************/

        /// <summary>
        /// Rendering is called at the end of every frame. Use it for everything rendering related.
        /// </summary>
        protected virtual void Rendering(float dt)
        {
            // Look at primary character
            LookAtTarget();

        }

        public void LookAtTarget()
        {
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = rotation;
        }

        public void LookAtTarget(Vector3 targetPosition)
        {
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = rotation;
        }

        Vector3 directionalVelocity = Vector3.forward;
        float directionalSlider = 0;

        private void FolowTarget(float dt)
        {

            Vector3 directionalVelocity =
                (kid.directional.velocity.magnitude > 0.1f)
                    ?
                    this.directionalVelocity = kid.directional.velocity
                    :
                    this.directionalVelocity;

            Quaternion targetRotation;
            Vector3 targetRotationEuler;
            Quaternion currentRotation;
            Vector3 currentRotationEuler;
            Vector3 finalRotationEuler;
            Quaternion finalRotation;

            targetRotation = Quaternion.LookRotation(-directionalVelocity, Vector3.up);
            targetRotationEuler = targetRotation.eulerAngles;
            currentRotation = transform.rotation;
            currentRotationEuler = currentRotation.eulerAngles;

            finalRotationEuler = new Vector3(
                currentRotation.x,
                targetRotationEuler.y,
                currentRotation.z
                );
            finalRotation = Quaternion.Euler(finalRotationEuler);

            ////////////////


            Debug.DrawRay(kid.transform.position, finalRotation * Vector3.forward * 5f, Color.cyan);

            Quaternion lookFromTargetTowardCamera = Quaternion.Euler(-around.y, -around.x, 0); //




            currentRotation = finalRotation;
            currentRotationEuler = currentRotation.eulerAngles;
            targetRotation = lookFromTargetTowardCamera;
            targetRotationEuler = targetRotation.eulerAngles;


            finalRotationEuler = new Vector3(
                targetRotationEuler.x,
                currentRotationEuler.y,
                0
                );
            finalRotation = Quaternion.Euler(finalRotationEuler);

            Debug.DrawRay(kid.transform.position, finalRotation * Vector3.forward * 5f, Color.red);

            const float distFromPlayer = 10;
            const float speed = 10;

            //Quaternion lookFromTargetTowardCamera = Quaternion.Euler(-around.y, -around.x, 0); // Quaternion.LookRotation(transform.position - primaryCharacterTransform.position, Vector3.up);
            lookFromTargetTowardCamera = finalRotation;
            Vector3 direction = finalRotation * Vector3.forward;

            Vector3 horizontalDirection = Vector3.ProjectOnPlane(direction, Vector3.up);
            Vector3 horizontalPosition = horizontalDirection * distFromPlayer;
            Vector3 verticalPosition = Vector3.up * 2;


            //Vector3 position = transform.position;
            //position = primaryCharacterTransform.position + direction * distFromPlayer;
            //position = Vector3.MoveTowards(transform.position, position, speed * dt);
            //transform.position = position;
            Vector3 position = kid.transform.position;
            position += horizontalPosition;
            position += verticalPosition;
            position = Vector3.MoveTowards(transform.position, position, speed * dt);
            transform.position = position;
        }

        /*********
         * LOGIC *
         *********/

        public Vector3 directionalTargetDirection = Vector3.right;

        public Vector2 GetAround(Vector3 target, Vector3 camera)
        {
            Quaternion lookFromCameraToCharacter = Quaternion.LookRotation(camera - target, Vector3.up);
            Vector3 cameraPositionAroundCharacter = lookFromCameraToCharacter.eulerAngles;
            //cameraPositionAroundCharacter += Vector3.one * 360;
            Vector2 output = new Vector3(
                -cameraPositionAroundCharacter.y,
                -cameraPositionAroundCharacter.x
                );
            // Repeat x in [0;360[
            while (output.x >= 360)
            {
                output.x -= 360;
            }
            while (output.x < 0)
            {
                output.x += 360;
            }
            // Repeat y in [0;360[
            while (output.y >= 360)
            {
                output.y -= 360;
            }
            while (output.y < 0)
            {
                output.y += 360;
            }
            return output;
        }

        public void PushCamera(Vector2 direction)
        {
            around += direction;
        }

        public Vector3 GetHorizontalPositionAroundCharacter(Vector3 position)
        {
            Vector3 localPositon = position - kid.transform.position;
            localPositon = Vector3.ProjectOnPlane(localPositon, Vector3.up);
            localPositon = localPositon.normalized * distanceAround;
            return localPositon;
        }

        public Vector3 GetVerticalPosition(Vector3 position)
        {
            Vector3 localPositon = position - kid.transform.position;
            localPositon = Vector3.Project(localPositon, Vector3.up);
            return localPositon;
        }

        public Vector3 GetTargetPosition()
        {
            Vector3 character = kid.transform.position;
            Vector3 ahead = character + directionalSlider * (targetDirectionalDirectionSmooth * 8f);
            Vector3 targetPosition = (ahead + character) / 2f;
            return targetPosition;
        }

        public Vector3 GetLocalHorizontalPositionAroundCharacter(Vector3 direction)
        {
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            return direction.normalized * distanceAround;
        }

        public Vector3 GetLocalVerticalPositionAroundCharacter(Vector3 localHeight)
        {
            localHeight = Vector3.Project(localHeight, Vector3.up);
            return localHeight;
        }

        public Vector3 GetCharacterTowardCameraDirection()
        {
            return (transform.position - kid.transform.position).normalized;
        }


        Vector3 targetPosition;
        public Vector3 targetDirectionalDirectionSmooth = Vector3.right;

        public void TickPositionAroundCharacter(float dt, bool instant = false)
        {
            //float angle = Vector3.Angle(directionalTargetDirection, targetDirectionalDirectionSmooth);
            //targetDirectionalDirectionSmooth =
            //    Vector3.RotateTowards(
            //        targetDirectionalDirectionSmooth,
            //        directionalTargetDirection,
            //        (angle * Mathf.Deg2Rad) * dt * 2f,
            //        0
            //        );

            float dist = Vector3.Distance(targetDirectionalDirectionSmooth, directionalTargetDirection);
            targetDirectionalDirectionSmooth = Vector3.MoveTowards(targetDirectionalDirectionSmooth, directionalTargetDirection, (dist + 0f) * dt * 6f);

            targetPosition = GetTargetPosition();


            Vector3 localPosition = Vector3.zero;
            TickHorizontalPositionAroundCharacter(dt, ref localPosition, instant);
            TickVerticalPositionAroundCharacter(dt, ref localPosition, instant);
            transform.position = kid.transform.position + localPosition;
        }

        private float _lastMagnitude;

        public void TickHorizontalPositionAroundCharacter(float dt, ref Vector3 localPosition, bool instant = false)
        {
            Vector3 localHorizontalPosition = GetLocalHorizontalPositionAroundCharacter(transform.position - kid.transform.position);
            float magnitude = Vector3.Project(kid.compass.normal, Vector3.up).magnitude;
            if (kid.climbing.IsClimbing() && magnitude < 0.8f)
            {
                Vector3 targetDirectionalPosition = GetLocalHorizontalPositionAroundCharacter(
                    kid.characterCompass.transform.up + Vector3.ProjectOnPlane(directionalTargetDirection, Vector3.up)
                    );
                float angle = Vector3.Angle(localHorizontalPosition, targetDirectionalPosition);

                if (!instant && angle < 130)
                {
                    localHorizontalPosition =
                    Vector3.RotateTowards(
                        localHorizontalPosition,
                        targetDirectionalPosition,
                        (angle * Mathf.Deg2Rad) * dt * 8f,
                        0
                        );
                }
                else
                {
                    localHorizontalPosition = targetDirectionalPosition;
                }
                _lastMagnitude = magnitude;
            }
            else
            {
                Vector3 targetDirectionalPosition = GetLocalHorizontalPositionAroundCharacter(-directionalTargetDirection);
                float angle = Vector3.Angle(localHorizontalPosition, targetDirectionalPosition);
                if (angle < 130)
                {
                    localHorizontalPosition =
                        Vector3.RotateTowards(
                            localHorizontalPosition,
                            targetDirectionalPosition,
                            (angle * Mathf.Deg2Rad) * dt * 1.5f,
                            0
                            );
                }
                _lastMagnitude = 0;
            }

            localPosition += localHorizontalPosition;
        }

        public void TickVerticalPositionAroundCharacter(float dt, ref Vector3 localPosition, bool instant = false)
        {
            Vector3 localVerticalPosition = GetLocalVerticalPositionAroundCharacter(transform.position - kid.transform.position);
            Vector3 targetVerticalPosition = Vector3.up * 5;

            float dist = Vector3.Distance(localVerticalPosition, targetVerticalPosition);
            localVerticalPosition = Vector3.MoveTowards(localVerticalPosition, targetVerticalPosition, (dist + 0f) * dt * 8f);


            localPosition += localVerticalPosition;
        }
    }
}