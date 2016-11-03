using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {

    [NonSerialized]
    protected CharacterController _characterController;

    public CharacterCamera _characterCamera;
    public CharacterCompass _characterCompass;

    protected Vector2 _directionalInput;

    public float _moveSpeed = 7.5f;


    public Vector3 _gravity;
    public Vector3 _gravityRotation;

    // Jump
    public bool _jumpInputDown;
    public bool _jumpInputUp;
    public float _maxJumpHeight = 4;
    public float _minJumpHeight = 1;
    public float _timeToJumpApex = .4f;
    Vector3 _jumpVelocity = Vector3.zero;
    float _maxJumpVelocity;
    float _minJumpVelocity;

    // Mobility
    public enum MobilityState {
        GROUNDED,
        AIRBORN,
        JUMPING,
        CLIMBING,
        LEAPING
    }
    public MobilityState mobilityState = MobilityState.AIRBORN;

    protected bool isClimbing = false;

    protected virtual void Awake() {
        _characterController = GetComponent<CharacterController>();
        // Setup jump
        _gravity.y = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity.y) * _timeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity.y) * _minJumpHeight);
    }

	protected virtual void Update() {
        // Input
        _directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        _jumpInputDown |= Input.GetButtonDown("Jump");
        if (Input.GetButtonDown("Jump")) {
            _jumpInputUp = false;
        }
        _jumpInputUp |= Input.GetButtonUp("Jump");
        if (Input.GetButtonUp("Jump")) {
            _jumpInputDown = false;
        }
    }

	protected virtual void FixedUpdate() {
        DefaultController();
    }

    protected void DefaultController() {
        // Gravity
        Vector3 gravity = _gravity;
        Quaternion gravityRotation = Quaternion.Euler(_gravityRotation);
        gravity = gravityRotation * gravity;

        // Directional
        Vector3 directionalVelocity = GetDirectionalVelocity();

        // Jump
        Vector3 jumpVelocity = GetJumpVelocity(gravityRotation);
        if (!IsClimbing()) {
            // Apply gravity if not climbing
            ApplyGravity(gravity);
        } else {
            // Reset jump velocity otherwise
            jumpVelocity = Vector3.zero;
        }

        // Global
        Vector3 globalVelocity = directionalVelocity + jumpVelocity;
        if (isClimbing) {
            globalVelocity = Vector3.zero;
        }

        CollisionFlags collisionFlags = _characterController.Move(globalVelocity * Time.fixedDeltaTime);

        // Refresh rotation
        if (_directionalInput.magnitude > 0.1 /* deadzone */) {
            Quaternion directionalRotation = Quaternion.LookRotation(directionalVelocity, -gravity);
            transform.rotation = directionalRotation;
        }

        if ((collisionFlags & CollisionFlags.Sides) != 0) {
            // Touching sides
        }

        if ((collisionFlags & CollisionFlags.Above) != 0) {
            // Touching ceiling
        }

        if ((collisionFlags & CollisionFlags.Below) != 0) {
            // Touching ground
            mobilityState |= MobilityState.GROUNDED;
            mobilityState &= ~MobilityState.AIRBORN;
            // Refresh jump velocity
            _jumpVelocity -= Vector3.Project(_jumpVelocity, gravity);
        } else {
            mobilityState |= MobilityState.AIRBORN;
            mobilityState &= ~MobilityState.GROUNDED;
        }

        if (_jumpInputDown) {
            if ((_characterController.collisionFlags & CollisionFlags.Below) != 0) {
                // Initialise jump
                _jumpVelocity.y = _maxJumpVelocity;
                _jumpInputDown = false;
            }
        }

        if (_jumpInputUp) {
            if (_jumpVelocity.y > _minJumpVelocity) {
                // Interupt jump
                _jumpVelocity.y = _minJumpVelocity;
                _jumpInputUp = false;
            }
        }
    }

    protected Vector3 GetDirectionalVelocity() {
        // Directional
        Quaternion forwardRotation = _characterCompass.transform.rotation;
        Vector3 directional = new Vector3(_directionalInput.x, 0, _directionalInput.y);
        directional = forwardRotation * directional;
        directional *= _moveSpeed;
        return directional;
    }

    protected Vector3 GetJumpVelocity(Quaternion gravityRotation) {
        // Jump
        return gravityRotation * _jumpVelocity;
    }

    protected void ApplyGravity(Vector3 gravity) {
        _jumpVelocity += gravity * Time.fixedDeltaTime;
    }

    protected virtual bool IsClimbing() {
        return false;
    }

    // Climbing
    [Serializable]
    public class ClimbingParameters {
        public float correctionSpeed = 10;
        public float ascendSpeed = 10;
        public float pushSpeed = 10;
    }
    public ClimbingParameters climbingParameters = new ClimbingParameters();

    public void Climb(Climbable climbableObject) {
        isClimbing = true;
        StartCoroutine(ClimbProcess(climbableObject));
    }

    public IEnumerator ClimbProcess(Climbable climbableObject) {
        // Correct player position
        Vector3 targetCorectedPosition = climbableObject.box.ClosestPointOnBounds(transform.position);
        transform.position = targetCorectedPosition;

        yield return new WaitForFixedUpdate();

        // Ascend
        Vector3 ascendedPosition = targetCorectedPosition + Vector3.up * 4;
        while(Vector3.Distance(transform.position, ascendedPosition) > 0.5f) {
            transform.position = Vector3.MoveTowards(transform.position, ascendedPosition, climbingParameters.ascendSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = ascendedPosition;


        yield return new WaitForFixedUpdate();

        // Push front 
        Vector3 pushedPosition = ascendedPosition - climbableObject.transform.forward * 1;
        while (Vector3.Distance(transform.position, pushedPosition) > 0.5f) {
            transform.position = Vector3.MoveTowards(transform.position, pushedPosition, climbingParameters.pushSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = pushedPosition;

        isClimbing = false;
        yield return null;
    }

}
