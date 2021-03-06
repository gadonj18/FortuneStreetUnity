﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The main game state that controls all aspects of players' turns
/// </summary>
public class Playing : BaseGameState {
	#region Members and Properties
	public enum States { Init, ChooseAction, Roll, Move, Confirm, FinishTurn };
	private States currentState = States.Init;
	public States CurrentState {
		get { return currentState; }
	}
	private Board board = new Board();
	public Board Board {
		get { return board; }
	}
	private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
	private int currentPlayerIdx;
	private GameObject playerObj;
	private PlayerController playerScript;
	public PlayerController PlayerScript {
		get { return playerScript; }
	}
	private int movesLeft = 0;
	private Sprite[] diceSprites;
	private List<List<Tile>> possiblePaths;
	private MoveList moveList = new MoveList();
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
	#endregion

	#region Unity Methods
	/// <summary>
	/// Called when the script instance is being loaded. Used to initialize any variables or game state before the game starts.
	/// Note: Called before Start()
	/// </summary>
	public void Awake() {
		UIManager.RollButtonClick += new UIManager.UIButtonHandler(RollButton_Click);
		UIManager.SellStockButtonClick += new UIManager.UIButtonHandler(SellStockButton_Click);
		UIManager.SellShopButtonClick += new UIManager.UIButtonHandler(SellShopButton_Click);
		DiceController.DiceSpun += new DiceController.DiceSpinHandler(DiceSpun);
		diceSprites = Resources.LoadAll<Sprite>(@"Sprites/UIDice");
	}

	/// <summary>
	/// Called every frame
	/// </summary>
	public void Update() {
		if(currentState == States.Move && movesLeft > 0) {
			UIManager.Instance.DiceMoves.GetComponent<Image>().sprite = diceSprites[movesLeft - 1];
		}
	}
	#endregion

	#region UI Events
	/// <summary>
	/// UI Event called when the Roll button is clicked from the main Action Menu.
	/// </summary>
	public void RollButton_Click(UIEventArgs e) {
		UIManager.Instance.ActionMenu.SetActive(false);
		movesLeft = Random.Range(1, 6);
		UIManager.Instance.Dice.GetComponent<Renderer>().enabled = true;
		StartCoroutine(UIManager.Instance.Dice.GetComponent<DiceController>().Roll(movesLeft));
	}

	/// <summary>
	/// UI Event when player is confirming the end of their turn.
	/// </summary>
	public void FinishMoveYes_Click(UIEventArgs e) {
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(this.FinishMoveYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(this.FinishMoveNo_Click);
		currentState = States.FinishTurn;
		playerScript.CurrentTile.PlayersOnTile.Add(playerScript);
		if(playerScript.CurrentTile.GetComponent<BaseTileActions>() != null) {
			StartCoroutine(playerScript.CurrentTile.GetComponent<BaseTileActions>().LandOnTile());
		} else {
			FinishTurn();
		}
	}

	/// <summary>
	/// UI Event when player is canceling the end of their turn.
	/// </summary>
	public void FinishMoveNo_Click(UIEventArgs e) {
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(this.FinishMoveYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(this.FinishMoveNo_Click);
		StartCoroutine("MovePlayerToTile", moveList.Moves[moveList.Moves.Count - 1].fromTile);
		UIManager.Instance.DiceMoves.GetComponent<Image>().sprite = diceSprites[0];
		UIManager.Instance.DiceMoves.GetComponent<Image>().enabled = true;
		UIManager.Instance.YesNoMenu.SetActive(false);
		currentState = States.Move;
	}
	#endregion

	#region Input Events
	/// <summary>
	/// Called by the Input Manager when the user clicks the mouse. Hands off click to method with the name "[STATE]_[MOUSECODE]Click()", e.g. "Move_LeftClick()"
	/// </summary>
	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		System.Reflection.MethodInfo method = typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click");
		if(method != null) method.Invoke(this, new object[] { e });
	}

	public override void MouseClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void MouseHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyUp(InputEventArgs e) { throw new System.NotImplementedException (); }

	/// <summary>
	/// Called when the user left clicks during the Move substate. Used to select a tile to move to.
	/// </summary>
	public void Move_LeftClick(InputEventArgs e) {
		Tile targetTile = board.GetTileAt(e.MousePosition);
		if(targetTile != null && targetTile != playerScript.CurrentTile) {
			StartCoroutine("MovePlayerToTile", targetTile);
		}
	}
	#endregion

	#region Player movement
	/// <summary>
	/// Event handler for the end of a dice spin.
	/// </summary>
	public void DiceSpun() {
		UIManager.Instance.Dice.GetComponent<Renderer>().enabled = false;
		UIManager.Instance.Dice.transform.localScale = UIManager.Instance.Dice.GetComponent<DiceController>().OrigScale;
		UIManager.Instance.DiceMoves.GetComponent<Image>().sprite = diceSprites[movesLeft - 1];
		UIManager.Instance.DiceMoves.GetComponent<Image>().enabled = true;
		possiblePaths = board.GetPaths(playerScript.CurrentTile, playerScript.Direction, movesLeft);
		foreach(List<Tile> path in possiblePaths) {
			path.Insert(0, playerScript.CurrentTile);
		}
		currentState = States.Move;
	}

	/// <summary>
	/// Upon clicking a tile, MovePlayerToTile will determine which path the player must take to get to the target tile and traverse it.
	/// </summary>
	private IEnumerator MovePlayerToTile(Tile targetTile) {
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

	/// <summary>
	/// If player has already moved past the target tile, reverse until the player is either at the tile or in the path to the tile.
	/// </summary>
	private IEnumerator ReverseToPath(List<Tile> path) {
		for(int i = moveList.Moves.Count - 1; i >= 0; i--) {
			if(board.TileInPath(path, playerScript.CurrentTile)) {
				break;
			}

			Move lastMove = moveList.Moves[i];
			yield return StartCoroutine(LeaveTile(playerScript.CurrentTile));
			yield return StartCoroutine(MoveToTile(lastMove.fromTile, true));
			yield return StartCoroutine(playerScript.MoveToTile(lastMove.fromTile));
			yield return StartCoroutine(OnTile(playerScript.CurrentTile));
			ReverseMoveChanges(lastMove);
			movesLeft++;
		}
	}

	/// <summary>
	/// When reversing, undo any changes that were done in the tile you passed.
	/// </summary>
	private void ReverseMoveChanges(Move move) {
		playerScript.Cash -= move.cash;
		playerScript.Level -= move.level;
		foreach(KeyValuePair<Constants.Suits, bool> cardChange in move.cards) {
			playerScript.Suits[cardChange.Key] = !cardChange.Value;
		}
		if(move.stock != null) playerScript.Stocks[((KeyValuePair<string, int>)move.stock).Key] -= ((KeyValuePair<string, int>)move.stock).Value;
		moveList.GoBack();
	}

	/// <summary>
	/// After MovePlayerToTile builds a queue of tiles to move to, move the player along this queue.
	/// </summary>
	private IEnumerator ProcessMoveQueue() {
		List<Tile> queue = new List<Tile>();
		queue.AddRange(moveList.Queue);
		foreach(Tile nextTile in queue) {
			Move newMove = new Move();
			newMove.fromTile = playerScript.CurrentTile;
			newMove.toTile = nextTile;
			if(movesLeft <= 1) UIManager.Instance.DiceMoves.GetComponent<Image>().enabled = false;
			yield return StartCoroutine(LeaveTile(newMove.fromTile));
			yield return StartCoroutine(MoveToTile(newMove.toTile));
			if(newMove.toTile.GetComponent<BaseTileActions>() != null) newMove.toTile.GetComponent<BaseTileActions>().MoveChanges(newMove);
			yield return StartCoroutine(playerScript.MoveToTile(nextTile));
			yield return StartCoroutine(OnTile(playerScript.CurrentTile));
			movesLeft--;
			moveList.Next(newMove);
		}
	}
	
	/// <summary>
	/// While a player is moving to a tile, the logic is handled in the tile's TileAction script's MoveToTile method.
	/// </summary>
	private IEnumerator MoveToTile(Tile tile, bool reversing = false) {
		if(tile.GetComponent<BaseTileActions>() != null) {
			yield return StartCoroutine(tile.GetComponent<BaseTileActions>().MoveToTile(reversing));
		}
	}
	
	/// <summary>
	/// When a player lands on a tile, the logic is handled in the tile's TileAction script's OnTile method.
	/// For example, to collect a suit, purchase stock, or level up at the bank.
	/// </summary>
	private IEnumerator OnTile(Tile tile) {
		if(tile.GetComponent<BaseTileActions>() != null) {
			yield return StartCoroutine(tile.GetComponent<BaseTileActions>().OnTile());
		}
	}

	/// <summary>
	/// When a player moves off a tile, the logic is handled in the previous tile's TileAction script's LeaveTile method.
	/// For example, to hide any UI elements that were shown as part of the previous tile's MoveToTile method.
	/// </summary>
	private IEnumerator LeaveTile(Tile tile) {
		tile.PlayersOnTile.Remove(playerScript);
		if(tile.GetComponent<BaseTileActions>() != null) {
			yield return StartCoroutine(tile.GetComponent<BaseTileActions>().LeaveTile());
		}
	}

	/// <summary>
	/// When a moving player is out of moves, present the Yes/No menu asking if they'd like to stop at this tile.
	/// </summary>
	private void ConfirmFinishMove() {
		UIManager.YesButtonClick += new UIManager.UIButtonHandler(this.FinishMoveYes_Click);
		UIManager.NoButtonClick += new UIManager.UIButtonHandler(this.FinishMoveNo_Click);
		UIManager.Instance.YesNoMenu.transform.FindChild("Title/Text").GetComponent<Text>().text = "Stop here?";
		UIManager.Instance.YesNoMenu.SetActive(true);
		currentState = States.Confirm;
	}
	#endregion

	#region State Events
	/// <summary>
	/// Set all members and UI elements to their starting states to begin the game.
	/// Note that GameObject references should be set up in the Awake method, not here.
	/// Assume that this method is being called as a game restart, so all references will already be set up.
	/// </summary>
	public override IEnumerator Starting() {
		BuildLevel(GameLogic.GetComponent<Game>().BoardInfo);
		UIManager.Instance.MiniMap.GetComponent<MiniMap>().GameState = this;
		UIManager.Instance.MiniMap.GetComponent<MiniMap>().BuildMiniMap(board);
		board.BuildPaths();
		AddPlayers(Config.Instance.playerInfo);
		currentPlayerIdx = 0;
		SwitchTurns(currentPlayerIdx);
		UIManager.Instance.ActionMenu.SetActive(true);
		//UIManager.Instance.OwnedPropertyInfo.SetActive(false);
		UIManager.Instance.UnownedPropertyInfo.SetActive(false);
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.Instance.Dice.GetComponent<Renderer>().enabled = false;
		UIManager.Instance.DiceMoves.GetComponent<Image>().enabled = false;
		UIManager.Instance.Message.SetActive(false);
		UIManager.Instance.SettleDebtMenu.SetActive(false);
		currentState = States.ChooseAction;
		UIManager.Instance.MiniMap.GetComponent<MiniMap>().SwitchTurns();
		yield break;
	}

	public override IEnumerator Ending() { yield return null; }

	/// <summary>
	/// Using the BoardInfo read from JSON when the game was started, construct the physical tiles representing the game board.
	/// </summary>
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
			if(info.Tiles[i].TileX > board.MaxX) board.MaxX = info.Tiles[i].TileX;
			if(info.Tiles[i].TileY > board.MaxY) board.MaxY = info.Tiles[i].TileY;
		}
		board.Districts = info.Districts;
		for(int i = 0; i < board.Districts.Count; i++) {
			StockPrices.Add(board.Districts[i], 10);
		}
	}

	/// <summary>
	/// Create players based on the information from the Config
	/// </summary>
	private void AddPlayers(List<PlayerInfo> playerInfo) {
		BoardInfo info = this.GameLogic.GetComponent<Game>().BoardInfo;
		for(var i = 0; i < playerInfo.Count; i++) {
		//foreach(PlayerInfo entry in playerInfo) {
			GameObject player = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Characters/LeftShark"));
			PlayerController playerScript = (PlayerController)player.GetComponent<PlayerController>();
			playerScript.gameLogic = this;
			playerScript.CurrentTile = board.bank.GetComponent<Bank>();
			playerScript.CurrentTile.PlayersOnTile.Add(playerScript);
			playerScript.transform.position = new Vector3(playerScript.CurrentTile.transform.position.x, playerScript.transform.position.y, playerScript.CurrentTile.transform.position.z - 0.491f);
			playerScript.PlayerName = playerInfo[i].Name;
			playerScript.Color = playerInfo[i].Color;
			playerScript.Cash = info.StartCash;
			StartCoroutine(playerScript.Hide());
			playerScript.ScoreUI = GameObject.Find("UIOverlay/PlayerScores/Player" + (i + 1));
			players.Add(i, player);

			GameObject playerScore = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/PlayerScore"));
			playerScore.transform.SetParent(UIManager.Instance.PlayerScores.transform);
			playerScore.name = "Player" + (i + 1);
			RectTransform rect = playerScore.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(0f, -100f + (50f * i) + 10f, 0f);
			rect.sizeDelta = new Vector2(0f, 50f);
			playerScore.GetComponent<Image>().color = playerScript.Color;
			playerScore.transform.FindChild("PlayerNameBG1").GetComponent<Image>().color = playerScript.Color;
			playerScore.transform.FindChild("PlayerNameBG2").GetComponent<Image>().color = playerScript.Color;
			playerScript.ScoreUI = playerScore;

		}
	}
	#endregion

	#region End Turn
	public void FinishTurn() {
		UIManager.Instance.UnownedPropertyInfo.SetActive(false);
		if(PlayerScript.Cash < 0) {
			UIManager.Instance.SettleDebtMenu.GetComponent<Renderer>().enabled = true;
		} else {
			currentPlayerIdx++;
			if(currentPlayerIdx > 3) currentPlayerIdx = 0;
			SwitchTurns(currentPlayerIdx);
		}
	}
	
	private void SellStockButton_Click(UIEventArgs e) {
		
	}
	
	private void SellShopButton_Click(UIEventArgs e) {
		
	}

	private void SwitchTurns(int playerNum) {
		if(playerScript != null) {
			StartCoroutine(playerScript.Hide());
		}
		playerObj = players[playerNum];
		playerScript = playerObj.GetComponent<PlayerController>();
		StartCoroutine(playerScript.Show());
		UIManager.Instance.ActionMenu.SetActive(true);
		currentState = States.ChooseAction;
		moveList = new MoveList();
		Camera.main.GetComponent<MoveCamera>().SwitchTarget(playerObj.transform);
		UIManager.Instance.MiniMap.GetComponent<MiniMap>().SwitchTurns();
	}
	#endregion
}