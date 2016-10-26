using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CharacterCamera : MonoBehaviour {

    [NonSerialized]
    private Camera _attachedCamera;
    public Character _attachedCharacter;

    // Parametters
    [Range(0,360)]
    public float _rotation;
    private const float _rotationSensitivity = 15f;
    [Range(0, 1)]
    public float _height;
    private const float _heightSensitivity = 0.075f;
    public bool reflectHeight = true;

    public float _distance;

    private void Awake() {
        _attachedCamera = GetComponent<Camera>();
    }

    private void Update() {
        // Input
        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Refresh rotation
        _rotation += input.x * _rotationSensitivity;
        _rotation = Mathf.Repeat(_rotation, 360);
        // Refresh height
        _height += (reflectHeight ? -input.y : input.y) * _heightSensitivity;
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
        Quaternion rotationOver = Quaternion.Euler(90 + _height* 180, 0, 0);
        Quaternion rotation = rotationAround * rotationOver;
        Vector3 direction = rotation * Vector3.forward;
        Vector3 position = _distance * direction;
        position += _attachedCharacter.transform.position;
        // Set position
        transform.position = position;

        // Refresh rotation
        rotation *= Quaternion.Euler(180, 0, 0);
        // Set rotation
        transform.rotation = rotation;
    }

}
