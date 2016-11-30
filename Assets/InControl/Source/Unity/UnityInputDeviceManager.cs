using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InControl.TinyJSON;
using UnityEngine;


namespace InControl
{
    public class UnityInputDeviceManager : InputDeviceManager {
        float deviceRefreshTimer = 0.0f;
        const float deviceRefreshInterval = 1.0f;
        List<InputDeviceProfile> systemDeviceProfiles = new List<InputDeviceProfile>();
        List<InputDeviceProfile> customDeviceProfiles = new List<InputDeviceProfile>();

        bool hasJoystickHash;
        int joystickHash;


        public UnityInputDeviceManager() {
            AddSystemDeviceProfiles();
            // LoadDeviceProfiles();
            AttachDevices();
            // Map based device change check (Initialisation)
            InitialisePlugedDeviceMap();
        }


        public override void Update(ulong updateTick, float deltaTime) {
            deviceRefreshTimer += deltaTime;

            // Map based device change check (Update)
            UpdatePlugedDeviceMap();
            
            /*
            // Legacy, hash based pluged device change check
            if (!hasJoystickHash || deviceRefreshTimer >= deviceRefreshInterval) {
                deviceRefreshTimer = 0.0f;
                if (joystickHash != JoystickHash) {
                    Logger.LogInfo("Change in attached Unity joysticks detected; refreshing device list.");
                    DetachDevices();
                    AttachDevices();
                }
            }
            */
        }


        void AttachDevices() {
            AttachKeyboardDevices();
            AttachJoystickDevices();
            joystickHash = JoystickHash;
            hasJoystickHash = true;
        }


        void DetachDevices() {
            var deviceCount = devices.Count;
            for (int i = 0; i < deviceCount; i++) {
                InputManager.DetachDevice(devices[i]);
            }
            devices.Clear();
        }


        public void ReloadDevices() {
            DetachDevices();
            AttachDevices();
        }


        void AttachDevice(UnityInputDevice device) {
            devices.Add(device);
            InputManager.AttachDevice(device);
        }


        void AttachKeyboardDevices() {
            int deviceProfileCount = systemDeviceProfiles.Count;
            for (int i = 0; i < deviceProfileCount; i++) {
                var deviceProfile = systemDeviceProfiles[i];
                if (deviceProfile.IsNotJoystick && deviceProfile.IsSupportedOnThisPlatform) {
                    AttachDevice(new UnityInputDevice(deviceProfile));
                }
            }
        }


        void AttachJoystickDevices() {
            try {
                var joystickNames = Input.GetJoystickNames();
                var joystickNameCount = joystickNames.Length;
                for (int i = 0; i < joystickNameCount; i++) {
                    DetectJoystickDevice(i + 1, joystickNames[i]);
                }
            } catch (Exception e) {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
            }
        }


        void DetectJoystickDevice(int unityJoystickId, string unityJoystickName) {
            if (unityJoystickName == "WIRED CONTROLLER" ||
                unityJoystickName == " WIRED CONTROLLER") {
                // Ignore Steam controller for now.
                return;
            }

            if (unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) != -1) {
                // Unity thinks some webcams are joysticks. >_<
                return;
            }

            // PS4 controller only works properly as of Unity 4.5
            if (InputManager.UnityVersion < new VersionInfo(4, 5)) {
                if (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer //||
                                                                      //Application.platform == RuntimePlatform.OSXWebPlayer // DEBUG
                    ) {
                    if (unityJoystickName == "Unknown Wireless Controller") {
                        // Ignore PS4 controller in Bluetooth mode on Mac since it connects but does nothing.
                        return;
                    }
                }
            }

            // As of Unity 4.6.3p1, empty strings on windows represent disconnected devices.
            if (InputManager.UnityVersion >= new VersionInfo(4, 6, 3)) {
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer // ||
                                                                          //Application.platform == RuntimePlatform.WindowsWebPlayer // DEBUG
                    ) {
                    if (String.IsNullOrEmpty(unityJoystickName)) {
                        return;
                    }
                }
            }

            InputDeviceProfile matchedDeviceProfile = null;

            if (matchedDeviceProfile == null) {
                matchedDeviceProfile = customDeviceProfiles.Find(config => config.HasJoystickName(unityJoystickName));
            }

            if (matchedDeviceProfile == null) {
                matchedDeviceProfile = systemDeviceProfiles.Find(config => config.HasJoystickName(unityJoystickName));
            }

            if (matchedDeviceProfile == null) {
                matchedDeviceProfile = customDeviceProfiles.Find(config => config.HasLastResortRegex(unityJoystickName));
            }

            if (matchedDeviceProfile == null) {
                matchedDeviceProfile = systemDeviceProfiles.Find(config => config.HasLastResortRegex(unityJoystickName));
            }

            InputDeviceProfile deviceProfile = null;

            if (matchedDeviceProfile == null) {
                deviceProfile = new UnknownUnityDeviceProfile(unityJoystickName);
                systemDeviceProfiles.Add(deviceProfile);
            } else {
                deviceProfile = matchedDeviceProfile;
            }

            if (!deviceProfile.IsHidden) {
                var joystickDevice = new UnityInputDevice(deviceProfile, unityJoystickId);
                AttachDevice(joystickDevice);

                if (matchedDeviceProfile == null) {
                    Logger.LogWarning("Device " + unityJoystickId + " with name \"" + unityJoystickName + "\" does not match any known profiles.");
                } else {
                    Logger.LogInfo("Device " + unityJoystickId + " matched profile " + deviceProfile.GetType().Name + " (" + deviceProfile.Name + ")");
                }
            } else {
                Logger.LogInfo("Device " + unityJoystickId + " matching profile " + deviceProfile.GetType().Name + " (" + deviceProfile.Name + ")" + " is hidden and will not be attached.");
            }
        }


        int JoystickHash {
            get {
                var joystickNames = Input.GetJoystickNames();
                var joystickNamesCount = joystickNames.Length;
                int hash = 17 * 31 + joystickNamesCount;
                for (int i = 0; i < joystickNamesCount; i++) {
                    hash = hash * 31 + joystickNames[i].GetHashCode();
                }
                return hash;
            }
        }


        void AddSystemDeviceProfile(UnityInputDeviceProfile deviceProfile) {
            if (deviceProfile.IsSupportedOnThisPlatform) {
                systemDeviceProfiles.Add(deviceProfile);
            }
        }


        void AddSystemDeviceProfiles() {
            foreach (var typeName in UnityInputDeviceProfileList.Profiles) {
                var deviceProfile = (UnityInputDeviceProfile)Activator.CreateInstance(Type.GetType(typeName));
                AddSystemDeviceProfile(deviceProfile);
            }
        }

        /*
		public void AddDeviceProfile( UnityInputDeviceProfile deviceProfile )
		{
			if (deviceProfile.IsSupportedOnThisPlatform)
			{
				customDeviceProfiles.Add( deviceProfile );
			}
		}


		public void LoadDeviceProfiles()
		{
			LoadDeviceProfilesFromPath( CustomProfileFolder );
		}


		public void LoadDeviceProfile( string data )
		{
			var deviceProfile = UnityInputDeviceProfile.Load( data );
			AddDeviceProfile( deviceProfile );
		}


		public void LoadDeviceProfileFromFile( string filePath )
		{
			var deviceProfile = UnityInputDeviceProfile.LoadFromFile( filePath );
			AddDeviceProfile( deviceProfile );
		}


		public void LoadDeviceProfilesFromPath( string rootPath )
		{
			if (Directory.Exists( rootPath ))
			{
				var filePaths = Directory.GetFiles( rootPath, "*.json", SearchOption.AllDirectories );
				foreach (var filePath in filePaths)
				{
					LoadDeviceProfileFromFile( filePath );
				}
			}
		}


		internal static void DumpSystemDeviceProfiles()
		{
			var filePath = CustomProfileFolder;
			Directory.CreateDirectory( filePath );

			foreach (var typeName in UnityInputDeviceProfileList.Profiles)
			{
				var deviceProfile = (UnityInputDeviceProfile) Activator.CreateInstance( Type.GetType( typeName ) );
				var fileName = deviceProfile.GetType().Name + ".json";
				deviceProfile.SaveToFile( filePath + "/" + fileName );
			}
		}


		static string CustomProfileFolder
		{
			get
			{
				return Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) + "/InControl/Profiles";
			}
		}
		/**/


        /** FIX ON PLUG/UNPLUG DEVICE
         * Pluged devices changes only affect the devices that have changed state, instead of disconecting and reconecting every devices.
         */
        
        bool[] plugedDeviceMap;
        
        void InitialisePlugedDeviceMap() {
            // Retrieve joystick names
            string[] joystickNames = Input.GetJoystickNames();

            // Make new a pluged map
            int joystickCount = joystickNames.Length;
            bool[] newDevicePlugedMap = new bool[joystickCount];
            for (int i = 0; i < joystickCount; ++i) {
                newDevicePlugedMap[i] = !string.IsNullOrEmpty(joystickNames[i]);
            }

            // Initialise map
            plugedDeviceMap = newDevicePlugedMap;
        }
        
        void UpdatePlugedDeviceMap() {
            // Retrieve joystick names
            string[] joystickNames = Input.GetJoystickNames();

            // Make new a pluged map
            int joystickCount = joystickNames.Length;
            bool[] newDevicePlugedMap = new bool[joystickCount];
            for (int i = 0; i < joystickCount; ++i) {
                newDevicePlugedMap[i] = !string.IsNullOrEmpty(joystickNames[i]);
            }

            // Check for detached devices
            for (int i = 0; i < Mathf.Min(plugedDeviceMap.Length, joystickCount); ++i) {
                if(
                    plugedDeviceMap[i] &&   // Device was attached
                    !newDevicePlugedMap[i]  // Device is now detached
                    ) {
                    // Find corresponding detached device
                    InputDevice detachedDevice = null;
                    foreach(var device in devices) {
                        if(device.SortOrder == i + 101) {
                            detachedDevice = device;
                            break;
                        }
                    }

                    // Remove device from InputManager if found
                    if (detachedDevice != null) {
                        InputManager.DetachDevice(detachedDevice);
                    }
                }
            }

            // Check for attached devices
            for (int i = 0; i < joystickCount; ++i) {
                if (
                    i + 1 > plugedDeviceMap.Length || // Detect device if array has grown
                    (
                        !plugedDeviceMap[i] &&  // Device was detached
                        newDevicePlugedMap[i]   // Device is now attached
                    )
                ) {
                    // Device was attached

                    // Add device to InputManager
                    DetectJoystickDevice(i + 1, joystickNames[i]);
                }
            }

            // Refresh pluged device map
            plugedDeviceMap = newDevicePlugedMap;

            // /* DEBUG */
            //string log = " ";
            //for (int i = 0; i < joystickCount; ++i) {
            //    log += "{" + (plugedDeviceMap[i] ? "X" : "O") + "} ";
            //}
            //Debug.Log(log);

        }

	}
}

