using UnityEngine;
using System.Collections;

public static class MouseLock
{
	private static bool mouseLocked;
    public static bool IsMobileControl = false;


    public static bool MouseLocked {
		get {
			return mouseLocked;
		}
		set {
            if (IsMobileControl)
            {
                mouseLocked = false;
                Screen.lockCursor = false;
            }
            else
            {
                mouseLocked = value;
                Screen.lockCursor = value;
            }
		}
	}
	

}

