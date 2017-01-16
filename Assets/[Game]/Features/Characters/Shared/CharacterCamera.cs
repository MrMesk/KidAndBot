using UnityEngine;
using System;
using System.Collections;

//[RequireComponent(typeof(Camera))]
public class CharacterCamera : MonoBehaviour {
    
    public Character _attachedCharacter;

    // Parametters
    [Range(0,360)]
    private float _rotationX = 0;
    [Header("Sensitivity")]
    public float _mouseRotationSensitivity = 10f;
    public float _stickRotationSensitivity = 7.5f;
    [Range(0, 1)]
    private float _heightY = 0.5f;
    public float _mouseHeightSensitivity = 0.075f;
    public float _stickHeightSensitivity = 0.050f;
    [Header("Reflection")]
    public bool _mouseReflectHeight = true;
    public bool _stickReflectHeight = false;
    [Header("Parameters")]
    public float _minDistance = 2;
    public float _maxDistance = 10;
	public LayerMask _checkMask;
    [Header("Parameters")]
    public AnimationCurve _distanceFromHeight = new AnimationCurve();
    
    // Input
    private PlayerInput.Controller input { get { return _attachedCharacter.input; } }  

    private void Update() 
	{
        // Input
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 stickInput = new Vector2(Input.GetAxis("Right Stick X"), Input.GetAxis("Right Stick Y"));
        mouseInput = Vector2.zero;
        stickInput = input.shared.camera.Value;

        // Refresh rotation
        float rotationDelta =
            // Mouse
            mouseInput.x * _mouseRotationSensitivity +
            // Stick
            stickInput.x * _stickRotationSensitivity;
        _rotationX += rotationDelta * Time.deltaTime;
        _rotationX = Mathf.Repeat(_rotationX, 360);

        // Refresh height
        float heightDelta =
            // Mouse
            (_mouseReflectHeight ? -mouseInput.y : mouseInput.y) * _mouseHeightSensitivity +
            // Stick
            (_stickReflectHeight ? -stickInput.y : stickInput.y) * _stickHeightSensitivity;
        _heightY += heightDelta * Time.deltaTime;
        _heightY = Mathf.Clamp01(_heightY);
    }

    private void LateUpdate() {
        // Don't refresh camera if it's not attached to any character
        if (_attachedCharacter == null) return;

        // Refresh camera position and rotation
        RefreshCamera();
    }

    public void RefreshCamera() 
	{
        // Refresh position
        Quaternion rotationX = Quaternion.Euler(0, _rotationX, 0);          // Rotation of the camera around the character
        Quaternion rotationY = Quaternion.Euler(89 + _heightY * 180, 0, 0); // Rotation of the camera over and under the character (89 instead of 90 for debug)
        Quaternion rotation = rotationX * rotationY;                        // Rotation of the camera
        Vector3 direction = rotation * Vector3.forward;                     // Direction pointing from character to the camera's wanted position

        float wantedDistance =      // Wanted distance from character to camera
            Mathf.Lerp(_minDistance, _maxDistance, _distanceFromHeight.Evaluate(_heightY));

        // Correct the distance to prevent the camera from entering geometry.
        float correctedDistance;    // Distance this camera should be to prevent it entering geometry.        
		RaycastHit hit;
		if (Physics.Raycast (_attachedCharacter.transform.position, transform.position - _attachedCharacter.transform.position, out hit, wantedDistance, _checkMask)) {
            // Move the camera away from the hit point
            const float cushion = 0.25f;
            correctedDistance = Mathf.MoveTowards(
                Vector3.Distance(_attachedCharacter.transform.position, hit.point),
                _minDistance,
                cushion
                );
        } else {
            correctedDistance = wantedDistance;
        }
		
        // Calculate the position of the camera
        Vector3 position = correctedDistance * direction;
        // Add one third of the character's height to the hight of this camera
		position += _attachedCharacter.transform.position + new Vector3(0, _attachedCharacter.controller.height/3);
        // Apply position
        transform.position = position;

        // Inverse rotation so it now points from camera to it's attached character
        rotation *= Quaternion.Euler(180, 0, 0);
        // Apply rotation
        transform.rotation = rotation;
    }

}
