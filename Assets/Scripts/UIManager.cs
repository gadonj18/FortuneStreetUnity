using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	public StateManager stateManager;

	public void RollButton_Click() {
		((Playing)stateManager.CurrentState).RollButton_Click();
	}
	
	public void YesButton_Click() {
		((Playing)stateManager.CurrentState).YesButton_Click();
	}
	
	public void NoButton_Click() {
		((Playing)stateManager.CurrentState).NoButton_Click();
	}
}