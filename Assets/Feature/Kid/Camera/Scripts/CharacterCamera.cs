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

        /// <summary>
        /// The unity camera that is used to render the scene.
        /// </summary>
        [System.NonSerialized]
        protected Camera _unityCamera = null;
        public Camera unityCamera
        {
            get
            {
                return _unityCamera ?? (_unityCamera = GetComponentInChildren<Camera>());
            }
        }

        /// <summary>
        /// Awake is called after this class's constructor.
        /// </summary>
        public void Awake()
        {
            // Bind camera
            _unityCamera = GetComponentInChildren<Camera>();
        }

        /// <summary>
        /// Unity Start
        /// </summary>
        public void Start()
        {
            if (Application.isPlaying)
            {
                Camera.onPreCull += OnRenderingPreCull;
            }

        }

        /// <summary>
        /// Update is called every frame. Use it for everything input related.
        /// </summary>
        private void Update()
        {
            // If the player is moving,
            if (kid.directional.velocity.magnitude > 0.1f)
            {
                // Then, refresh the direction pointing ahead of the player
                directionFacedByPlayer = kid.directional.velocity.normalized;
            }
        }

        /// <summary>
        /// OnRenderingPreCull is called before the camera starts the culling process. Use it for recalibrate the camera before rendering.
        /// </summary>
        private void OnRenderingPreCull(Camera camera = null)
        {
            // Rendering
            Rendering(Time.deltaTime);
        }
        
        // Container transform
        [Header("Configuration")]
        public KidCharacter kid;

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
            
            // Tick directional direction slider
            if (kid.directional.velocity.magnitude > 0.1f)
            {
                float dist = 1 - sliderLookTowardDirectionFacedByPlayer;
                sliderLookTowardDirectionFacedByPlayer = Mathf.MoveTowards(sliderLookTowardDirectionFacedByPlayer, 1, dist * dt);
            }
            else
            {
                float dist = sliderLookTowardDirectionFacedByPlayer - 0;
                sliderLookTowardDirectionFacedByPlayer = Mathf.MoveTowards(sliderLookTowardDirectionFacedByPlayer, 0, dist * dt * 0.25f);
            }

            //
            TickPositionAroundCharacter(dt);

        }


        /*********
         * LOGIC *
         *********/

        /// <summary>
        /// The distance from the player the camera will circulate around.
        /// </summary>
        [System.NonSerialized]
        public float distanceAround = 10;

        /// <summary>
        /// The direction the player is currently facing.
        /// </summary>
        public Vector3 directionFacedByPlayer = Vector3.right;
        /// <summary>
        /// The smoothed direction the player is facing.
        /// </summary>
        public Vector3 directionFacedByPlayerSmoothed = Vector3.right;
        /// <summary>
        /// Slider indicating how much the camera should look toward the direction the player is currently facing.
        /// </summary>
        float sliderLookTowardDirectionFacedByPlayer = 0;

        /// <summary>
        /// Returns the position in world space the camera should look at.
        /// </summary>
        public Vector3 GetTargetPosition()
        {
            // Look at the character
            Vector3 character = kid.transform.position;

            // Look at the direction the character is facing
            const float aheadDist = 8f; // How far in that direction the camera shoold look
            Vector3 ahead = character + (directionFacedByPlayerSmoothed * sliderLookTowardDirectionFacedByPlayer * aheadDist);

            // Make an avreage of both
            Vector3 targetPosition = (ahead + character) / 2f;

            // Return it
            return targetPosition;
        }

        /// <summary>
        /// Convert a direction to a local position around the character.
        /// </summary>
        public Vector3 GetLocalHorizontalPositionAroundCharacter(Vector3 direction)
        {
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            return direction.normalized * distanceAround;
        }

        /// <summary>
        /// Convert a local position to a local height around the character.
        /// </summary>
        public Vector3 GetLocalVerticalPositionAroundCharacter(Vector3 localHeight)
        {
            localHeight = Vector3.Project(localHeight, Vector3.up);
            return localHeight;
        }

        /// <summary>
        /// Return the direction originating from the character pointing toard the camera.
        /// </summary>
        public Vector3 GetCharacterTowardCameraDirection()
        {
            return (transform.position - kid.transform.position).normalized;
        }

        // Logic Tick

        /// <summary>
        /// Tick the position of the camera around the character.
        /// </summary>
        /// <param name="dt">Time elapsed since the previous tick</param>
        /// <param name="instant"></param>
        public void TickPositionAroundCharacter(float dt, bool instant = false)
        {
            float dist = Vector3.Distance(directionFacedByPlayerSmoothed, directionFacedByPlayer);
            directionFacedByPlayerSmoothed = Vector3.MoveTowards(
                directionFacedByPlayerSmoothed,
                directionFacedByPlayer,
                (dist + 0f) * dt *
                (kid.IsJumping() ? 1f : 4f)
                );

            Vector3 localPosition = Vector3.zero;
            TickHorizontalPositionAroundCharacter(dt, ref localPosition, instant);
            TickVerticalPositionAroundCharacter(dt, ref localPosition, instant);
            transform.position = kid.transform.position + localPosition;
        }

        //private float _lastMagnitude;

        public void TickHorizontalPositionAroundCharacter(float dt, ref Vector3 localPosition, bool instant = false)
        {
            Vector3 localHorizontalPosition = GetLocalHorizontalPositionAroundCharacter(transform.position - kid.transform.position);
            float magnitude = Vector3.Project(kid.compass.normal, Vector3.up).magnitude;
            if (kid.climbing.IsClimbing() && magnitude < 0.8f)
            {
                Vector3 targetDirectionalPosition = GetLocalHorizontalPositionAroundCharacter(
                    Vector3.ProjectOnPlane(kid.compass.transform.up, Vector3.up).normalized +
                    Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(directionFacedByPlayer, kid.compass.transform.up), Vector3.up) * 0.5f
                    );
                float angle = Vector3.Angle(localHorizontalPosition, targetDirectionalPosition);

                if (angle < 130)
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
                   // localHorizontalPosition = targetDirectionalPosition;
                    localHorizontalPosition =
                    Vector3.RotateTowards(
                        localHorizontalPosition,
                        targetDirectionalPosition,
                        (angle * Mathf.Deg2Rad) * dt * 24f,
                        0
                        );
                }
            }
            else
            {
                Vector3 targetDirectionalPosition = GetLocalHorizontalPositionAroundCharacter(-directionFacedByPlayer);
                float angle = Vector3.Angle(localHorizontalPosition, targetDirectionalPosition);
                if (angle < 130)
                {
                    localHorizontalPosition =
                        Vector3.RotateTowards(
                            localHorizontalPosition,
                            targetDirectionalPosition,
                            (angle * Mathf.Deg2Rad) * dt * (kid.IsJumping()? 0.5f : 2f) * sliderLookTowardDirectionFacedByPlayer,
                            0
                            );
                }
            }

            localPosition += localHorizontalPosition;
        }

        public void TickVerticalPositionAroundCharacter(float dt, ref Vector3 localPosition, bool instant = false)
        {
            Vector3 localVerticalPosition = GetLocalVerticalPositionAroundCharacter(transform.position - kid.transform.position);
            Vector3 targetVerticalPosition = Vector3.up * 5;

            float dist = Vector3.Distance(localVerticalPosition, targetVerticalPosition);
            localVerticalPosition = Vector3.MoveTowards(
                localVerticalPosition,
                targetVerticalPosition,
                (dist + 0f) * dt * (kid.IsJumping() && kid.activeJump.HasReachedPeak() ? 0.5f : 8f)
                );


            localPosition += localVerticalPosition;
        }


        /*************
         * RENDERING *
         *************/

        /// <summary>
        /// Rendering is called at the end of every frame. Use it for everything rendering related.
        /// </summary>
        protected virtual void Rendering(float dt)
        {
            // Look at primary character before rendering
            LookAtTarget();

        }

        public void LookAtTarget()
        {
            LookAtTarget(GetTargetPosition());
        }

        public void LookAtTarget(Vector3 targetPosition)
        {
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = rotation;
        }



        
        /*********
         * LOGIC *
         *********/
        
    }
}