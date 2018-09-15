using UnityEngine;

namespace CnControls
{
	/// <summary>
	/// Virtual button class
	/// </summary>
	public class VirtualButton
	{
		/// <summary>
		/// Name of the button for which this virtual button has to be registered
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Is this button currently pressed?
		/// </summary>
		public bool IsPressed { get; private set; }

		/// <summary>
		/// The last frame this button was pressed
		/// </summary>
		private int _lastPressedFrame = -1;

		/// <summary>
		/// The last frame this butto was released
		/// </summary>
		private int _lastReleasedFrame = -1;

		public VirtualButton(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Press logic sets the current state of the button to "IsPressed" untill the Release() method is called
		/// </summary>
		public void Press()
		{
			if (IsPressed)
			{
				return;
			}
			IsPressed = true;
			_lastPressedFrame = Time.frameCount;
		}

		/// <summary>
		/// Release logic frees the button from its "IsPressed" state
		/// </summary>
		public void Release()
		{
			IsPressed = false;
			_lastReleasedFrame = Time.frameCount;
		}

		/// <summary>
		/// Is this button currently pressed?
		/// </summary>
		public bool GetButton
		{
			get { return IsPressed; }
		}

		/// <summary>
		/// Check whether this button has just been pressed 
		/// </summary>
		public bool GetButtonDown
		{
			get
			{
				return _lastPressedFrame != -1 && _lastPressedFrame - Time.frameCount == -1;
			}
		}

		/// <summary>
		/// Check whether this button has just been released 
		/// </summary>
		public bool GetButtonUp
		{
			get
			{
				return _lastReleasedFrame != -1 && _lastReleasedFrame == Time.frameCount - 1;
			}
		}
	}
}