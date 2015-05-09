using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	public delegate void UIButtonHandler(UIEventArgs e);
	public static event UIButtonHandler RollButtonClick;
	public static event UIButtonHandler YesButtonClick;
	public static event UIButtonHandler NoButtonClick;

	public StateManager stateManager;

	public void RollButton_Click() {
		if(RollButtonClick != null) RollButtonClick(new UIEventArgs());
	}
	
	public void YesButton_Click() {
		if(YesButtonClick != null) YesButtonClick(new UIEventArgs());
	}
	
	public void NoButton_Click() {
		if(NoButtonClick != null) NoButtonClick(new UIEventArgs());
	}
}

public class UIEventArgs : System.EventArgs {
	private float time;
	public float Time {
		get { return time; }
	}

	public UIEventArgs() {
		time = UnityEngine.Time.unscaledTime;
	}
}