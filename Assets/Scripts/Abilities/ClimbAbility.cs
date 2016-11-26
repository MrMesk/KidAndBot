using UnityEngine;
using System.Collections;

public class ClimbAbility : MonoBehaviour {


    // State
    private Character character;

    void Start() {
        // Retrieve required component(s)
        character = GetComponentInParent<Character>();
        if (character == null) {
            this.enabled = false;
            return;
        }

        // Initialise jump
        InitialiseClimb();
    }

    private void InitialiseClimb() {

    }

    void FixedUpdate() {
        //Debug.Log(character.IsClimbing());
        if (!character.IsClimbing()) {
            return;
        }
        return;

        Vector3 climbCorrectionVector = -character.characterCompass.climbNormal * 2f;

        Debug.DrawRay(transform.position, climbCorrectionVector, Color.red);

        character.transform.rotation = Quaternion.LookRotation(character.characterCompass.transform.forward, character.characterCompass.climbNormal);

        // Apply to global velocity
        character.globalVelocity += climbCorrectionVector;

    }
}
