using UnityEngine;
using System;
using System.Collections;

//[RequireComponent(typeof(Camera))]
public class CharacterCamera : MonoBehaviour {
    
    public Character _attachedCharacter;

    // Parametters
    [Range(0,360)]
    private float _rotation = 0;
    [Header("Sensitivity")]
    public float _mouseRotationSensitivity = 10f;
    public float _stickRotationSensitivity = 7.5f;
    [Range(0, 1)]
    private float _height = 0.5f;
    public float _mouseHeightSensitivity = 0.075f;
    public float _stickHeightSensitivity = 0.050f;
    [Header("Reflection")]
    public bool _mouseReflectHeight = true;
    public bool _stickReflectHeight = false;
    [Header("Parameters")]
    public float _minDistance = 2;
    public float _maxDistance = 10;
    [Header("Parameters")]
    public AnimationCurve _distanceFromHeight = new AnimationCurve();
    
    protected virtual void Awake() { }

    private void Update() {
        // Input
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 stickInput = new Vector2(Input.GetAxis("Right Stick X"), Input.GetAxis("Right Stick Y"));

        // Refresh rotation
        float rotationDelta =
            // Mouse
            mouseInput.x * _mouseRotationSensitivity +
            // Stick
            stickInput.x * _stickRotationSensitivity;
        _rotation += rotationDelta * Time.deltaTime;
        _rotation = Mathf.Repeat(_rotation, 360);

        // Refresh height
        float heightDelta =
            // Mouse
            (_mouseReflectHeight ? -mouseInput.y : mouseInput.y) * _mouseHeightSensitivity +
            // Stick
            (_stickReflectHeight ? -stickInput.y : stickInput.y) * _stickHeightSensitivity;
        _height += heightDelta * Time.deltaTime;
        _height = Mathf.Clamp01(_height);
    }

    private void FixedUpdate() {

    }

    private void LateUpdate() {
        if(_attachedCharacter == null) {
            return;
        }
        RefreshCamera();
    }

    public void RefreshCamera() {
        // Refresh position
        Quaternion rotationAround = Quaternion.Euler(0, _rotation, 0);
        Quaternion rotationOver = Quaternion.Euler(89 + _height * 180, 0, 0); // 89 instead of 90 (debug)
        Quaternion rotation = rotationAround * rotationOver;
        Vector3 direction = rotation * Vector3.forward;
        float distance = Mathf.Lerp(_minDistance, _maxDistance, _distanceFromHeight.Evaluate(_height));
        Vector3 position = distance * direction;
        position += _attachedCharacter.transform.position;
        // Set position
        transform.position = position;

        // Refresh rotation
        rotation *= Quaternion.Euler(180, 0, 0);
        // Set rotation
        transform.rotation = rotation;
    }

}
