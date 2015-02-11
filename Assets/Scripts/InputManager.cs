using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public delegate void MouseHandler(InputEventArgs e);
	public delegate void KeyHandler(InputEventArgs e);
	public static event MouseHandler MouseClick;
	public static event MouseHandler MouseHeld;
	public static event MouseHandler MouseUp;
	public static event KeyHandler KeyClick;
	public static event KeyHandler KeyHeld;
	public static event KeyHandler KeyUp;

	void Update () {
		if(Input.GetMouseButtonDown(0)) { if(MouseClick != null) MouseClick(new InputEventArgs(0)); }
		if(Input.GetMouseButton(0)) { if(MouseHeld != null) MouseHeld(new InputEventArgs(0)); }
		if(Input.GetMouseButtonUp(0)) { if(MouseUp != null) MouseUp(new InputEventArgs(0)); }
		if(Input.GetMouseButtonDown(1)) { if(MouseClick != null) MouseClick(new InputEventArgs(1)); }
		if(Input.GetMouseButton(1)) { if(MouseHeld != null) MouseHeld(new InputEventArgs(1)); }
		if(Input.GetMouseButtonUp(1)) { if(MouseUp != null) MouseUp(new InputEventArgs(1)); }
		if(Input.GetMouseButtonDown(2)) { if(MouseClick != null) MouseClick(new InputEventArgs(2)); }
		if(Input.GetMouseButton(2)) { if(MouseHeld != null) MouseHeld(new InputEventArgs(2)); }
		if(Input.GetMouseButtonUp(2)) { if(MouseUp != null) MouseUp(new InputEventArgs(2)); }
	}
}

public class InputEventArgs : System.EventArgs {
	private int mouseCode;
	private string keyCode;
	private Vector3 mousePosition = Input.mousePosition;

	public float Time {
		get { return UnityEngine.Time.unscaledTime; }
	}

	public int MouseCode {
		get { return mouseCode; }
	}

	public string KeyCode {
		get { return keyCode; }
	}

	public Vector3 MousePosition {
		get { return mousePosition; }
	}
	
	public InputEventArgs(int mouseCode) {
		this.mouseCode = mouseCode;
	}

	public InputEventArgs(string keyCode) {
		this.keyCode = keyCode;
	}
}