using UnityEngine;
using System.Collections;

public class ClimbableObject : MonoBehaviour {

    public MeshRenderer meshRenderer;
    public Material hitMat;

    private Material backupMat;

    public Collider _collider;

    private void Start() {
        backupMat = meshRenderer.material;
        _collider = GetComponent<Collider>();
    }

    public void SetHitMat() {
        meshRenderer.material = hitMat;
    }
    public void CleanHitMat() {
        meshRenderer.material = backupMat;
    }

}
