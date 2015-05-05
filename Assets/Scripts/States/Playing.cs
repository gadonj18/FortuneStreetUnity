using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The main game state that controls all aspects of players' turns
/// </summary>
public class Playing : BaseGameState {
	private enum States { ChooseAction, Roll, Move, Confirm, FinishTurn };
	private States currentState;
	private Board board;
	private Dictionary<int, GameObject> players;
	private GameObject playerObj;
	private PlayerController playerScript;
	private int movesLeft = 0;
	private GameObject dice;
	private GameObject UIDiceMoves;
	private Sprite[] diceSprites;
	private List<List<Tile>> possiblePaths;
	private MoveList moveList;
	private GameObject ActionMenu;
	private GameObject YesNoMenu;
	/// <summary>
	/// List of input events we wish to register with the input manager
	/// </summary>
	public override ICollection<Constants.InputEvents> InputEvents {
		get { return inputEvents; }
	}
	private ICollection<Constants.InputEvents> inputEvents = new List<Constants.InputEvents>() {
		Constants.InputEvents.MouseUp
	};

	public void Awake() {
		currentState = States.ChooseAction;
		board = new Board();
		players = new Dictionary<int, GameObject>();
		playerObj = null;
		diceSprites = Resources.LoadAll<Sprite>(@"Sprites/UIDice");
	}

	public void Update() {
		if(currentState == States.Move && movesLeft > 0) {
			UIDiceMoves.GetComponent<Image>().sprite = diceSprites[movesLeft - 1];
		}
	}

	/// <summary>
	/// UI Event called when the Roll button is clicked from the main Action Menu
	/// </summary>
	public void RollButton_Click() {
		ActionMenu.SetActive(false);
		movesLeft = Random.Range(1, 6);
		movesLeft = 6;
		dice.GetComponent<Renderer>().enabled = true;
		StartCoroutine(dice.GetComponent<DiceController>().Roll(movesLeft));
	}
	
	public void YesButton_Click() {
		currentState = States.FinishTurn;
	}
	
	public void NoButton_Click() {
		currentState = States.Move;
		StartCoroutine("MoveToTile", moveList.Moves[moveList.Moves.Count - 1].fromTile);
		UIDiceMoves.GetComponent<Image>().sprite = diceSprites[0];
		UIDiceMoves.GetComponent<Image>().enabled = true;
		YesNoMenu.SetActive(false);
	}

	public void DiceSpun() {
		dice.GetComponent<Renderer>().enabled = false;
		UIDiceMoves.GetComponent<Image>().sprite = diceSprites[movesLeft - 1];
		UIDiceMoves.GetComponent<Image>().enabled = true;
		possiblePaths = board.GetPaths(playerScript.CurrentTile, playerScript.Direction, movesLeft);
		foreach(List<Tile> path in possiblePaths) {
			path.Insert(0, playerScript.CurrentTile);
		}
		currentState = States.Move;
	}

	/// <summary>
	/// Called by the Input Manager when the user clicks the mouse. Hands off click to method with the name "[STATE]_[MOUSECODE]Click()", e.g. "Move_LeftClick()"
	/// </summary>
	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		System.Reflection.MethodInfo method = typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click");
		if(method != null) method.Invoke(this, new object[] { e });
	}
	
	public void Move_LeftClick(InputEventArgs e) {
		Tile targetTile = board.GetTileAt(e.MousePosition);
		if(targetTile != null && targetTile != playerScript.CurrentTile) {
			StartCoroutine("MoveToTile", targetTile);
		}
	}

	/// <summary>
	/// Upon clicking a tile, MoveToTile will determine which path the player must take to get to the target tile and build a list of moves.
	/// </summary>
	/// <returns>The to tile.</returns>
	/// <param name="targetTile">Target tile.</param>
	private IEnumerator MoveToTile(Tile targetTile) {
		//Path from turn's starting tile to targetTile, not neccessarily full path containing targetTile
		List<Tile> path = board.GetPathToTile(possiblePaths, targetTile);
		if(path.Count == 0) yield break;

		//Let the player finish moving before interrupting
		while(playerScript.moving) {
			yield return null;
		}
		StopCoroutine("ReverseToPath");
		StopCoroutine("ReverseToTile");
		StopCoroutine("ProcessMoveQueue");

		//Tile you're on is NOT in the new path
		if(moveList.Moves.Count > 0 && !board.TileInPath(path, playerScript.CurrentTile)) {
			//Reverse to intersect with the new path
			yield return StartCoroutine("ReverseToPath", path);
		}

		if(path.Count > moveList.Moves.Count) {
			//Add remainder of path that has not already been moved to to the queue
			List<Tile> newQueue = new List<Tile>();
			for(int i = moveList.Moves.Count; i < path.Count; i++) {
				if(path[i] != playerScript.CurrentTile) {
					newQueue.Add(path[i]);
				}
			}
			moveList.AddQueue(newQueue);

			//And move to the queue
			yield return StartCoroutine("ProcessMoveQueue");
		}
		if(movesLeft == 0) FinishMove();
	}

	private IEnumerator ReverseToPath(List<Tile> path) {
		for(int i = moveList.Moves.Count - 1; i >= 0; i--) {
			if(board.TileInPath(path, playerScript.CurrentTile)) {
				break;
			}

			Move lastMove = moveList.Moves[i];
			yield return StartCoroutine(playerScript.MoveToTile(lastMove.fromTile));
			ReverseMoveChanges(lastMove);
			movesLeft++;
		}
	}
	
	private void ReverseMoveChanges(Move move) {
		playerScript.Cash -= move.cash;
		playerScript.Level -= move.level;
		foreach(KeyValuePair<Constants.Cards, bool> cardChange in move.cards) {
			playerScript.Cards[cardChange.Key] = !cardChange.Value;
		}
		if(move.stock != null) playerScript.Stocks[((KeyValuePair<string, int>)move.stock).Key] -= ((KeyValuePair<string, int>)move.stock).Value;
		moveList.GoBack();
	}

	private IEnumerator ProcessMoveQueue() {
		List<Tile> queue = new List<Tile>();
		queue.AddRange(moveList.Queue);
		foreach(Tile nextTile in queue) {
			Move newMove = new Move();
			newMove.fromTile = playerScript.CurrentTile;
			newMove.toTile = nextTile;
			yield return StartCoroutine(playerScript.MoveToTile(nextTile));
			movesLeft--;
			PassTile(newMove);
			moveList.Next(newMove);
		}
	}
	
	private void PassTile(Move move) {
		
	}
	
	private void FinishMove() {
		UIDiceMoves.GetComponent<Image>().enabled = false;
		YesNoMenu.transform.FindChild("Title").FindChild("Text").GetComponent<Text>().text = "Stop here?";
		YesNoMenu.SetActive(true);
		currentState = States.Confirm;
	}

	public override IEnumerator Starting() {
		Game script = this.GameLogic.GetComponent<Game>();
		BoardInfo info = script.BoardInfo;
		BuildLevel(info);
		board.BuildPaths();
		AddPlayers(Config.Instance.playerInfo);
		SwitchPlayers(1);
		Camera.main.GetComponent<MoveCamera>().Character = playerObj;
		ActionMenu = GameObject.Find("ActionMenu");
		ActionMenu.SetActive(true);
		YesNoMenu = GameObject.Find("YesNoMenu");
		YesNoMenu.SetActive(false);
		dice = GameObject.Find("Dice");
		dice.GetComponent<Renderer>().enabled = false;
		DiceController.DiceSpun += new DiceController.DiceSpinHandler(this.DiceSpun);
		UIDiceMoves = GameObject.Find("DiceMoves");
		UIDiceMoves.GetComponent<Image>().enabled = false;
		moveList = new MoveList();
		yield return null;
	}

	public override IEnumerator Ending() { yield return null; }
	public override void MouseClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void MouseHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyUp(InputEventArgs e) { throw new System.NotImplementedException (); }

	private void BuildLevel(BoardInfo info) {
		for(int i = 0; i < info.Tiles.Count; i++) {
			GameObject newTile = TileFactory.Instance.Build(info.Tiles[i].Code, info.Tiles[i].TileX, info.Tiles[i].TileY, info.Tiles[i].District);
			board.AddTile(newTile, info.Tiles[i].TileX, info.Tiles[i].TileY);
		}
		board.Districts = info.Districts;
	}

	private void AddPlayers(Dictionary<int,PlayerInfo> playerInfo) {
		foreach(KeyValuePair<int, PlayerInfo> entry in playerInfo) {
			GameObject player = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Characters/LeftShark"));
			PlayerController playerScript = (PlayerController)player.GetComponent<PlayerController>();
			playerScript.CurrentTile = board.bank.GetComponent<Bank>();
			playerScript.PlayerName = entry.Value.Name;
			playerScript.Color = entry.Value.Color;
			playerScript.Hide();
			players.Add(entry.Key, player);
		}
	}

	private void SwitchPlayers(int playerNum) {
		if(playerObj != null) {
			playerObj.GetComponent<PlayerController>().Hide();
		}
		playerObj = players[playerNum];
		playerScript = playerObj.GetComponent<PlayerController>();
		playerScript.Show();
		Camera.main.GetComponent<MoveCamera>().Character = playerObj;
	}
}