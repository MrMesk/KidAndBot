using UnityEngine;
using System.Collections;
using InControl;


namespace PlayerInput.Configuration {
    public class Bot : MonoBehaviour {
        // Punch
        [SerializeField]
        public TriggerInput punch;
        // Grab
        [SerializeField]
        public TriggerInput grab;
        // Pull
        [SerializeField]
        public TriggerInput pull;
        // Bump
        [SerializeField]
        public TriggerInput bump;
    }
}