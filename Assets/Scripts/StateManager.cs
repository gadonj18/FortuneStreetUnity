﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {
	private GameObject gameLogic;
    private IGameState currentState = null;
	private Constants.GameStates state;

	public IGameState CurrentState {
		get { return currentState; }
		set { currentState = value; }
	}

	public void Start() {
		gameLogic = GameObject.Find("_GameLogic");
	}

	public void ChangeState(string newState) {
		StartCoroutine("Transition", (Constants.GameStates)System.Enum.Parse(typeof(Constants.GameStates), newState));
	}

	private IEnumerator Transition(Constants.GameStates newState) {
		yield return null;
		if (currentState != null) {
			yield return StartCoroutine(currentState.Ending());
			UnregisterInputEvents();
			Destroy(gameLogic.GetComponent(state.ToString()));
		}
		currentState = null;
		state = newState;
		//currentState = (IGameState)System.Activator.CreateInstance(System.Type.GetType(newState.ToString()));
		//currentState = (IGameState)UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameLogic, "Assets/Scripts/StateManager.cs (33,30)", newState.ToString());
		currentState = (IGameState)gameLogic.AddComponent(System.Type.GetType(newState.ToString()));
		currentState.GameLogic = gameLogic;
		RegisterInputEvents();
		yield return StartCoroutine(currentState.Starting());
	}

	public void RegisterInputEvents() {
		foreach (Constants.InputEvents inputEvent in currentState.InputEvents) {
			switch(inputEvent) {
			case Constants.InputEvents.MouseClick:
				InputManager.MouseClick += new InputManager.MouseHandler(currentState.MouseClick);
				break;
			case Constants.InputEvents.MouseHeld:
				InputManager.MouseHeld += new InputManager.MouseHandler(currentState.MouseHeld);
				break;
			case Constants.InputEvents.MouseUp:
				InputManager.MouseUp += new InputManager.MouseHandler(currentState.MouseUp);
				break;
			case Constants.InputEvents.KeyClick:
				InputManager.KeyClick += new InputManager.KeyHandler(currentState.KeyClick);
				break;
			case Constants.InputEvents.KeyHeld:
				InputManager.KeyHeld += new InputManager.KeyHandler(currentState.KeyHeld);
				break;
			case Constants.InputEvents.KeyUp:
				InputManager.KeyUp += new InputManager.KeyHandler(currentState.KeyUp);
				break;
			}
		}
	}

	public void UnregisterInputEvents() {
		foreach (Constants.InputEvents inputEvent in currentState.InputEvents) {
			switch(inputEvent) {
			case Constants.InputEvents.MouseClick:
				InputManager.MouseClick -= new InputManager.MouseHandler(currentState.MouseClick);
				break;
			case Constants.InputEvents.MouseHeld:
				InputManager.MouseHeld -= new InputManager.MouseHandler(currentState.MouseHeld);
				break;
			case Constants.InputEvents.MouseUp:
				InputManager.MouseUp -= new InputManager.MouseHandler(currentState.MouseUp);
				break;
			case Constants.InputEvents.KeyClick:
				InputManager.KeyClick -= new InputManager.KeyHandler(currentState.KeyClick);
				break;
			case Constants.InputEvents.KeyHeld:
				InputManager.KeyHeld -= new InputManager.KeyHandler(currentState.KeyHeld);
				break;
			case Constants.InputEvents.KeyUp:
				InputManager.KeyUp -= new InputManager.KeyHandler(currentState.KeyUp);
				break;
			}
		}
	}
}