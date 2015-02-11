using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Game : MonoBehaviour {
	private BoardInfo boardInfo;
	//private InputManager inputManager;
	private StateManager stateManager;

	public BoardInfo BoardInfo {
		get { return boardInfo; }
		private set { boardInfo = value; }
	}

	//Init self
	void Awake() {
		//ChangeStates(Constants.GameStates.Playing);
		boardInfo = LoadLevel(1);
		Debug.Log("Loaded");
	}

	//Init others
	void Start () {
		GameObject logic = GameObject.Find("_GameLogic");
		//inputManager = logic.GetComponent<InputManager>();
		stateManager = logic.GetComponent<StateManager>();
		StartPlaying();
	}

	void StartPlaying() {
		stateManager.ChangeState("Playing");
	}

	BoardInfo LoadLevel(int BoardID) {
		TextAsset BoardFile = (TextAsset)Resources.Load("Boards/board" + BoardID, typeof(TextAsset));
		JSONNode jsonText = JSON.Parse(BoardFile.text);
		BoardInfo boardInfo = BoardInfo.FromJSON(jsonText);
		return boardInfo;
	}
}