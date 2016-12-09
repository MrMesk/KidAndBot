using UnityEngine;
using System.Collections;
using InControl;


namespace PlayerInput.Configuration {
    public class Kid : MonoBehaviour
	{
        // Jump
        [SerializeField]
        public TriggerInput jump = new TriggerInput();
		[SerializeField]
		public TriggerInput shoot = new TriggerInput();
	}
}