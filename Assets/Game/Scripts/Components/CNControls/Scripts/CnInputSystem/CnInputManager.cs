using System.Collections.Generic;
using UnityEngine;

namespace CnControls
{
    /// <summary>
    /// Common input manager class
    /// Can be used instead of Input logic, as it replicates the standard behaviour but adds additional logic
    /// </summary>
    public class CnInputManager
    {
        private static CnInputManager _instance;

        private static CnInputManager Instance
        {
            get { return _instance ?? (_instance = new CnInputManager()); }
        }

        private CnInputManager() { }

        /// <summary>
        /// Dictionary of virtual axis
        /// Every axis can be mapped to a number of actual virtual axis, 
        /// as with standard Unity Input system where you can create different buttons for, say, "Horizontal" axis
        /// </summary>
        private Dictionary<string, List<VirtualAxis>> _virtualAxisDictionary =
            new Dictionary<string, List<VirtualAxis>>();

        /// <summary>
        /// Dictionary of virtual buttons
        /// Every button can be mapped to a number of actual virtual buttons, 
        /// as with standard Unity Input system where you can create different buttons for, say, "Jump" button
        /// </summary>
        private Dictionary<string, List<VirtualButton>> _virtualButtonsDictionary =
            new Dictionary<string, List<VirtualButton>>();
        
        /// <summary>
        /// Additional logic for touch retreival
        /// It's possible to add some reflection-based emulated touches
        /// </summary>
        public static int TouchCount
        {
            get
            {
                return Input.touchCount;
            }
        }

        /// <summary>
        /// Additional logic for touch retreival
        /// It's possible to add some reflection-based emulated touches
        /// </summary>
        public static Touch GetTouch(int touchIndex)
        {
            return Input.GetTouch(touchIndex);
        }

        /// <summary>
        /// GetAxis method for getting current values for any desired axis
        /// </summary>
        /// <param name="axisName">The name of the axis to get value from</param>
        /// <returns>
        /// Current value of FIRST NON ZERO axis that are registered for that name
        /// ZERO if non if the virtual axis are being tweaked
        /// </returns>
        public static float GetAxis(string axisName)
        {
            return GetAxis(axisName, false);
        }

        /// <summary>
        /// "Copy" of the Input.GetAxisRaw method
        /// </summary>
        /// <param name="axisName">The name of the axis to get value from</param>
        /// <returns>
        /// Current value of FIRST NON ZERO axis that are registered for that name
        /// ZERO if non if the virtual axis are being tweaked
        /// </returns>
        public static float GetAxisRaw(string axisName)
        {
            return GetAxis(axisName, true);
        }

        /// <summary>
        /// Common private method for getting the axis values
        /// </summary>
        /// <param name="axisName">The name of the axis to get value from</param>
        /// <param name="isRaw">Whether the method sould return the raw value of the axis</param>
        /// <returns></returns>
        private static float GetAxis(string axisName, bool isRaw)
        {
            // If we have the axis registered as virtual, we call the retreival logic
            if (AxisExists(axisName))
            {
                return GetVirtualAxisValue(Instance._virtualAxisDictionary[axisName], axisName, isRaw);
            }

            // If we don't have the desired virtual axis registered, we just fallback to the default Unity Input behaviour
            return isRaw ? Input.GetAxisRaw(axisName) : Input.GetAxis(axisName);
        }

        /// <summary>
        /// Method for retrieval of the desired button pressed state
        /// </summary>
        /// <param name="buttonName">The name of the desired button</param>
        /// <returns>Is the button being currently pressed?</returns>
        public static bool GetButton(string buttonName)
        {
            // We first check the stadard Button behaviour
            var standardInputButtonState = Input.GetButton(buttonName);
            // If the stadard Unity Input button is being pressed, we just retur true
            if (standardInputButtonState == true) return true;

            // If not, we check our virtual buttons
            if (ButtonExists(buttonName))
            {
                return GetAnyVirtualButton(Instance._virtualButtonsDictionary[buttonName]);;
            }

            // If there is no such button registered, we return false;
            return false;
        }

        /// <summary>
        /// Method for retrieval of the desired button "has just been pressed" state
        /// </summary>
        /// <param name="buttonName">The name of the desired button</param>
        /// <returns>Is the button has just been pressed?</returns>
        public static bool GetButtonDown(string buttonName)
        {
            // We first check the stadard Button behaviour
            var standardInputButtonState = Input.GetButtonDown(buttonName);
            // If the stadard Unity Input button is being pressed, we just retur true
            if (standardInputButtonState == true) return true;

            // If not, we check our virtual buttons
            if (ButtonExists(buttonName))
            {
                return GetAnyVirtualButtonDown(Instance._virtualButtonsDictionary[buttonName]);
            }

            // If there is no such button registered, we return false;
            return false;
        }

        /// <summary>
        /// Method for retrieval of the desired button "has just been released" state
        /// </summary>
        /// <param name="buttonName">The name of the desired button</param>
        /// <returns>Is the button has just been released?</returns>
        public static bool GetButtonUp(string buttonName)
        {
            // We first check the stadard Button behaviour
            var standardInputButtonState = Input.GetButtonUp(buttonName);
            // If the stadard Unity Input button is being pressed, we just retur true
            if (standardInputButtonState == true) return true;

            // If not, we check our virtual buttons
            if (ButtonExists(buttonName))
            {
                return GetAnyVirtualButtonUp(Instance._virtualButtonsDictionary[buttonName]);
            }

            // If there is no such button registered, we return false;
            return false;
        }

        /// <summary>
        /// Check whether the specified axis exists
        /// </summary>
        /// <param name="axisName">Name of the axis to check</param>
        /// <returns>Does this axis exist?</returns>
        public static bool AxisExists(string axisName)
        {
            return Instance._virtualAxisDictionary.ContainsKey(axisName);
        }

        /// <summary>
        /// Check whether the specified button exists
        /// </summary>
        /// <param name="buttonName">Name of the button to check</param>
        /// <returns>Does this button exist?</returns>
        public static bool ButtonExists(string buttonName)
        {
            return Instance._virtualButtonsDictionary.ContainsKey(buttonName);
        }

        /// <summary>
        /// Registers the provided virtual axis
        /// </summary>
        /// <param name="virtualAxis">Virtual axis to register</param>
        public static void RegisterVirtualAxis(VirtualAxis virtualAxis)
        {
            // If it's the first such virtual axis, create a new list for that axis name
            if (!Instance._virtualAxisDictionary.ContainsKey(virtualAxis.Name))
            {
                Instance._virtualAxisDictionary[virtualAxis.Name] = new List<VirtualAxis>();
            }

            Instance._virtualAxisDictionary[virtualAxis.Name].Add(virtualAxis);
        }

        /// <summary>
        /// Unregisters the provided virtual axis
        /// </summary>
        /// <param name="virtualAxis">Virtual axis to unregister</param>
        public static void UnregisterVirtualAxis(VirtualAxis virtualAxis)
        {
            // If it's the first such virtual axis, create a new list for that axis name
            if (Instance._virtualAxisDictionary.ContainsKey(virtualAxis.Name))
            {
                if (!Instance._virtualAxisDictionary[virtualAxis.Name].Remove(virtualAxis))
                {
                    Debug.LogError("Requested axis " + virtualAxis.Name + " exists, but there's no such virtual axis that you're trying to unregister");                    
                }
            }
            else
            {
                Debug.LogError("Trying to unregister an axis " + virtualAxis.Name + " that was never registered");
            }
        }

        /// <summary>
        /// Registers the provided virtual button
        /// </summary>
        /// <param name="virtualButton">Virtual button to register</param>
        public static void RegisterVirtualButton(VirtualButton virtualButton)
        {
            if (!Instance._virtualButtonsDictionary.ContainsKey(virtualButton.Name))
            {
                Instance._virtualButtonsDictionary[virtualButton.Name] = new List<VirtualButton>();
            }

            Instance._virtualButtonsDictionary[virtualButton.Name].Add(virtualButton);
        }

        /// <summary>
        /// Unregisters the provided virtual button
        /// </summary>
        /// <param name="virtualButton">Virtual button to unregister</param>
        public static void UnregisterVirtualButton(VirtualButton virtualButton)
        {
            if (Instance._virtualButtonsDictionary.ContainsKey(virtualButton.Name))
            {
                if (!Instance._virtualButtonsDictionary[virtualButton.Name].Remove(virtualButton))
                {
                    Debug.LogError("Requested button axis exists, but there's no such virtual button that you're trying to unregister");
                }
            }
            else
            {
                Debug.LogError("Trying to unregister a button that was never registered");
            }
        }

        /// <summary>
        /// Private method that get's the value of the first non-zero virtual axis, registered with the specified name
        /// </summary>
        /// <param name="virtualAxisList">List of virtual axis to search through</param>
        /// <param name="axisName">Name of the axis (for the standard Input behaviour)</param>
        /// <param name="isRaw">Whether the method should return the Raw value of the axis</param>
        /// <returns></returns>
        private static float GetVirtualAxisValue(List<VirtualAxis> virtualAxisList, string axisName, bool isRaw)
        {
            // The method is really straightforward here
            // First, we check the standard Input.GetAxis method
            // If it's not zero, we return the value
            // If it IS zero, we return first non-zero value of any of the passed virtual axis
            // Or zero if all of them are zero

            float axisValue = isRaw ? Input.GetAxisRaw(axisName) : Input.GetAxis(axisName);
            if (!Mathf.Approximately(axisValue, 0f))
            {
                return axisValue;
            }

            for (int i = 0; i < virtualAxisList.Count; i++)
            {
                var currentAxisValue = virtualAxisList[i].Value;
                if (!Mathf.Approximately(currentAxisValue, 0f))
                {
                    return currentAxisValue;
                }
            }

            return 0f;
        }

        /// <summary>
        /// Simple logic for checking whether is any of the virtual buttons has been just pressed
        /// </summary>
        /// <param name="virtualButtons">Virtual buttons to search through</param>
        /// <returns>Is any of the buttons has just been pressed?</returns>
        private static bool GetAnyVirtualButtonDown(List<VirtualButton> virtualButtons)
        {
            for (int i = 0; i < virtualButtons.Count; i++)
            {
                if (virtualButtons[i].GetButtonDown) return true;
            }

            return false;
        }

        /// <summary>
        /// Simple logic for checking whether is any of the virtual buttons has been just released
        /// </summary>
        /// <param name="virtualButtons">Virtual buttons to search through</param>
        /// <returns>Is any of the buttons has just been released?</returns>
        private static bool GetAnyVirtualButtonUp(List<VirtualButton> virtualButtons)
        {
            for (int i = 0; i < virtualButtons.Count; i++)
            {
                if (virtualButtons[i].GetButtonUp) return true;
            }

            return false;
        }

        /// <summary>
        /// Simple logic for checking whether is any of the virtual buttons is currently pressed
        /// </summary>
        /// <param name="virtualButtons">Virtual buttons to search through</param>
        /// <returns>Is any of the buttons currently pressed?</returns>
        private static bool GetAnyVirtualButton(List<VirtualButton> virtualButtons)
        {
            for (int i = 0; i < virtualButtons.Count; i++)
            {
                if (virtualButtons[i].GetButton) return true;
            }

            return false;
        }

    }
}

