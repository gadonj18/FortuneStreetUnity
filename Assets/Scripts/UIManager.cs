using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	public StateManager stateManager;

	public void RollButton_Click() {
		((Playing)stateManager.CurrentState).RollButton_Click();
	}
}