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
	public PlayerController PlayerScript {
		get { return playerScript; }
	}
	private int movesLeft = 0;
	private GameObject dice;
	private GameObject UIDiceMoves;
	private Sprite[] diceSprites;
	private List<List<Tile>> possiblePaths;
	private MoveList moveList;
	private GameObject ActionMenu;
	private GameObject YesNoMenu;
	public GameObject OwnedPropertyInfo;
	public GameObject UnownedPropertyInfo;
	public Dictionary<string, int> StockPrices = new Dictionary<string, int>();
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
		movesLeft = 1;
		dice.GetComponent<Renderer>().enabled = true;
		StartCoroutine(dice.GetComponent<DiceController>().Roll(movesLeft));
	}
	
	public void YesButton_Click() {
		YesNoMenu.SetActive(false);
		currentState = States.FinishTurn;
		if(playerScript.CurrentTile.GetComponent<BaseTileActions>() != null) {
			StartCoroutine(playerScript.CurrentTile.GetComponent<BaseTileActions>().LandOnTile());
		}
	}
	
	public void NoButton_Click() {
		StartCoroutine("MoveToTile", moveList.Moves[moveList.Moves.Count - 1].fromTile);
		UIDiceMoves.GetComponent<Image>().sprite = diceSprites[0];
		UIDiceMoves.GetComponent<Image>().enabled = true;
		YesNoMenu.SetActive(false);
		currentState = States.Move;
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
		if(movesLeft == 0) ConfirmFinishMove();
	}

	private IEnumerator ReverseToPath(List<Tile> path) {
		for(int i = moveList.Moves.Count - 1; i >= 0; i--) {
			if(board.TileInPath(path, playerScript.CurrentTile)) {
				break;
			}

			Move lastMove = moveList.Moves[i];
			yield return StartCoroutine(LeaveTile(playerScript.CurrentTile));
			yield return StartCoroutine(PassTile(lastMove.fromTile, true));
			yield return StartCoroutine(playerScript.MoveToTile(lastMove.fromTile));
			ReverseMoveChanges(lastMove);
			movesLeft++;
		}
	}
	
	private void ReverseMoveChanges(Move move) {
		playerScript.Cash -= move.cash;
		playerScript.Level -= move.level;
		foreach(KeyValuePair<Constants.Suits, bool> cardChange in move.cards) {
			playerScript.Suits[cardChange.Key] = !cardChange.Value;
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
			yield return StartCoroutine(LeaveTile(newMove.fromTile));
			yield return StartCoroutine(PassTile(newMove.toTile));
			if(newMove.toTile.GetComponent<BaseTileActions>() != null) newMove.toTile.GetComponent<BaseTileActions>().MoveChanges(newMove);
			yield return StartCoroutine(playerScript.MoveToTile(nextTile));
			movesLeft--;
			moveList.Next(newMove);
		}
	}
	
	private IEnumerator PassTile(Tile tile, bool reversing = false) {
		if(tile.GetComponent<BaseTileActions>() != null) {
			yield return StartCoroutine(tile.GetComponent<BaseTileActions>().PassTile(reversing));
		}
	}
	
	private IEnumerator LeaveTile(Tile tile) {
		if(tile.GetComponent<BaseTileActions>() != null) {
			yield return StartCoroutine(tile.GetComponent<BaseTileActions>().LeaveTile());
		}
	}
	
	private void ConfirmFinishMove() {
		UIDiceMoves.GetComponent<Image>().enabled = false;
		YesNoMenu.transform.FindChild("Title/Text").GetComponent<Text>().text = "Stop here?";
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
		//OwnedPropertyInfo = GameObject.Find("OwnedPropertyInfo");
		//OwnedPropertyInfo.SetActive(false);
		UnownedPropertyInfo = GameObject.Find("UnownedPropertyInfo");
		UnownedPropertyInfo.SetActive(false);
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
			GameObject newTile;
			if(info.Tiles[i].Code == Constants.TileCodes.Property) {
				newTile = TileFactory.Instance.Build(info.Tiles[i].Code, info.Tiles[i].TileX, info.Tiles[i].TileY, info.Tiles[i].ShopValue, info.Tiles[i].District, info.Tiles[i].PropertyName);
			} else {
				newTile = TileFactory.Instance.Build(info.Tiles[i].Code, info.Tiles[i].TileX, info.Tiles[i].TileY);
			}
			if(newTile.GetComponent<BaseTileActions>() != null) {
				newTile.GetComponent<BaseTileActions>().GameState = this;
			}
			board.AddTile(newTile, info.Tiles[i].TileX, info.Tiles[i].TileY);
		}
		board.Districts = info.Districts;
		for(int i = 0; i < board.Districts.Count; i++) {
			StockPrices.Add(board.Districts[i], 10);
		}
	}

	private void AddPlayers(Dictionary<int,PlayerInfo> playerInfo) {
		BoardInfo info = this.GameLogic.GetComponent<Game>().BoardInfo;
		foreach(KeyValuePair<int, PlayerInfo> entry in playerInfo) {
			GameObject player = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Characters/LeftShark"));
			PlayerController playerScript = (PlayerController)player.GetComponent<PlayerController>();
			playerScript.gameLogic = this;
			playerScript.CurrentTile = board.bank.GetComponent<Bank>();
			playerScript.PlayerName = entry.Value.Name;
			playerScript.Color = entry.Value.Color;
			playerScript.Cash = info.StartCash;
			playerScript.Hide();
			playerScript.ScoreUI = GameObject.Find("UIOverlay/PlayerScores/Player" + entry.Key);
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