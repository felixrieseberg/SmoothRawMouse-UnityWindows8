using UnityEngine;

public class MouseLook : MonoBehaviour
{
	Vector2 _mouseAbsolute;
	Vector2 _smoothMouse;
	public Vector2 clampInDegrees = new Vector2 (360, 180);
	public bool lockCursor;
	public Vector2 sensitivity = new Vector2 (2, 2);
	public Vector2 smoothing = new Vector2 (3, 3);
	private float stunY;
	public Vector2 targetDirection;
	public bool noise;
	
	// Windows 8
	#if NETFX_CORE && !UNITY_EDITOR
	public static float windowsMouseDeltaRawX;
	public static float windowsMouseDeltaRawY;
	#endif
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		// Set target direction to the camera's initial orientation.
		targetDirection = transform.rotation.eulerAngles;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (!Screen.showCursor) {
			// Ensure the cursor is always locked when set
			Screen.lockCursor = true;
	
			// Allow the script to clamp based on a desired target value.
			Quaternion targetOrientation = Quaternion.Euler (targetDirection); 
			
			#if NETFX_CORE && !UNITY_EDITOR
			// Get raw mouse input for a cleaner reading on more sensitive mice.
			var mouseDelta = new Vector2 (windowsMouseDeltaRawX, windowsMouseDeltaRawY);
			windowsMouseDeltaRawX = 0;
			windowsMouseDeltaRawY = 0;
			#else
			// Get raw mouse input for a cleaner reading on more sensitive mice.
			var mouseDelta = new Vector2 (Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));			
			#endif
			
			// Scale input against the sensitivity setting and multiply that against the smoothing value.
			mouseDelta = Vector2.Scale (mouseDelta, new Vector2 (sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
	
			// Interpolate mouse movement over time to apply smoothing delta.
			_smoothMouse.x = Mathf.Lerp (_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
			_smoothMouse.y = Mathf.Lerp (_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
	
			// Find the absolute mouse movement value from point zero.
			_mouseAbsolute += _smoothMouse;
	
			// Gun Stun
			stunY += (0 - stunY) / 20f;
			
			// Clamp and apply the local x value first, so as not to be affected by world transforms.
			if (clampInDegrees.x < 360) {
				_mouseAbsolute.x = Mathf.Clamp (_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
			}
			var xRotation = Quaternion.AngleAxis (-_mouseAbsolute.y - stunY, targetOrientation * Vector3.right);
			transform.localRotation = xRotation; 
	
			// Then clamp and apply the global y value.
	
			if (clampInDegrees.y < 360)
				_mouseAbsolute.y = Mathf.Clamp (_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f); 
	
			var yRotation = Quaternion.AngleAxis (_mouseAbsolute.x, transform.InverseTransformDirection (Vector3.up));
			transform.localRotation *= yRotation;
			transform.rotation *= targetOrientation;
		}
	}
	
	public void Stun(float strength) {
		stunY = strength;
	}
	
	void OnGUI() {
		if (Debug.isDebugBuild) {
			#if NETFX_CORE && !UNITY_EDITOR
			GUI.Label(new Rect( Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), windowsMouseDeltaRawX.ToString() + " " + windowsMouseDeltaRawY.ToString());
			#else
			GUI.Label(new Rect( Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), Input.GetAxisRaw("Mouse X").ToString() + " " + Input.GetAxisRaw("Mouse Y").ToString());	
			#endif			
		}
	} 

}