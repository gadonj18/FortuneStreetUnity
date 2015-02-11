using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseGameState : IGameState {
	private GameObject gameLogic;
	public GameObject GameLogic {
		get {
			return gameLogic;
		}
		set {
			gameLogic = value;
		}
	}
	public abstract ICollection<Constants.InputEvents> InputEvents { get; }

	private Constants.GameStates state;
	public Constants.GameStates State {
		get { return state; }
		set { state = value; }
	}

	public abstract void MouseClick(InputEventArgs e);
	public abstract void MouseHeld(InputEventArgs e);
	public abstract void MouseUp(InputEventArgs e);
	public abstract void KeyClick(InputEventArgs e);
	public abstract void KeyHeld(InputEventArgs e);
	public abstract void KeyUp(InputEventArgs e);
	public abstract IEnumerator Start();
	public abstract IEnumerator End();
}