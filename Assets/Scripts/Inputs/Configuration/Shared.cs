using UnityEngine;
using System.Collections;
using InControl;


namespace PlayerInput.Configuration {
    public class Shared : MonoBehaviour {
        // Directional
        [SerializeField]
        public TwoAxisInput directional = new TwoAxisInput();
        // Camera
        [SerializeField]
        public new TwoAxisInput camera = new TwoAxisInput();
    }
}