using UnityEngine;
using System.Collections;

public class SwapableCharacterCamera : MonoBehaviour{


    [Header("Configuration")]
    public float timeToSwap = 0.25f;

    [Space(8)]

    [Header("State")]
    private Camera previewslySelectedCamera;
    public Camera currentlySelectedCamera;
    [Range(0,1)]
    public float sliderBetween = 1;

    public void OnValidate() {
        if (currentlySelectedCamera != null)
            SelectCamera(currentlySelectedCamera);
    }

    public void LateUpdate() {
        sliderBetween = timeToSwap > 0 ?
            Mathf.MoveTowards(sliderBetween, 1, (1 / timeToSwap) * Time.deltaTime) :
            1;
        Lerp(sliderBetween);
    }

    public void SelectCamera(Camera camera) {
        previewslySelectedCamera = currentlySelectedCamera;
        currentlySelectedCamera = camera;
        sliderBetween = 0;
    }

    public void Lerp(float t) {
        if (previewslySelectedCamera == null)
            sliderBetween = 1;
        if (previewslySelectedCamera == currentlySelectedCamera)
            sliderBetween = 1;

        transform.position = t < 1 ?
            Vector3.Lerp(
                previewslySelectedCamera.transform.position,
                currentlySelectedCamera.transform.position,
                t) :
            currentlySelectedCamera.transform.position;

        transform.rotation = t < 1 ?
            Quaternion.Lerp(
                previewslySelectedCamera.transform.rotation,
                currentlySelectedCamera.transform.rotation,
                t) :
            currentlySelectedCamera.transform.rotation;
    }

}
