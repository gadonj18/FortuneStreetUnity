using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Playing : BaseGameState {
	private enum States { ChooseAction, Roll, Move, Confirm };
	private States currentState;
	private ICollection<Constants.InputEvents> inputEvents;
	private Board board;
	private Dictionary<int, GameObject> players;
	private GameObject currentPlayer;
	private Player playerScript;
	private int movesLeft = 0;
	private GameObject ActionMenu;
	private GameObject Dice;
	private List<List<Tile>> paths;
	private MoveList moveList;

	public override ICollection<Constants.InputEvents> InputEvents {
		get { return inputEvents; }
	}

	public void Awake() {
		currentState = States.ChooseAction;
		inputEvents = new List<Constants.InputEvents>() {
			Constants.InputEvents.MouseUp
		};
		board = new Board();
		players = new Dictionary<int, GameObject>();
		currentPlayer = null;
	}

	public void RollButton_Click() {
		ActionMenu.SetActive(false);
		movesLeft = Random.Range(1, 6);
		movesLeft = 3;
		GameObject dice = GameObject.Find("Dice");
		dice.renderer.enabled = true;
		dice.GetComponent<DiceSpin>().Roll(movesLeft);
	}

	public void DiceSpun() {
		Dice.renderer.enabled = false;
		paths = board.GetPaths(playerScript.CurrentTile, playerScript.Direction, movesLeft);
		currentState = States.Move;
	}

	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		System.Reflection.MethodInfo method = typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click");
		if(method != null) method.Invoke(this, new object[] { e });
	}
	
	public void Move_LeftClick(InputEventArgs e) {
		//Get newPath
		//If queue
		//	reverseToPath(newPath)
		//	AddToQueue(newPath - oldPath)
		//else
		//	AddToQueue(newPath)
		//end
		//ProcessQueue

		Tile targetTile = GetTileAt(e.MousePosition);
		if(targetTile != null) {
			StartCoroutine("MoveToTile", targetTile);
		}

		/*if(!playerScript.moving) {
			Tile targetTile = GetTileAt(e.MousePosition);
			if(targetTile != null) {
				bool inPath = false;
				List<Tile> newPath = new List<Tile>();
				foreach(List<Tile> path in paths) {
					newPath = TileInPath(playerScript.CurrentTile, path, targetTile);
					if(newPath != null) {
						inPath = true;
						break;
					}
				}
				if(inPath) {
					moveList.ClearQueue();
					moveList.AddQueue(newPath);
					try {
						StopCoroutine("ProcessMoveQueue");
					} catch (UnityException ex) {

					}
					StartCoroutine("ProcessMoveQueue");
				}
			}
		}*/
	}

	private IEnumerator MoveToTile(Tile targetTile) {
		List<Tile> path = GetPathTo(targetTile);
		if(path.Count > 0) {
			if(moveList.Moves.Count > 0) {
				Debug.Log("Must reverse");
				yield return StartCoroutine("ReversetoPath", path);
			} else {
				moveList.AddQueue(path);
			}
			StartCoroutine("ProcessMoveQueue");
		}
	}

	private List<Tile> GetPathTo(Tile targetTile) {
		List<Tile> newPath = new List<Tile>();
		foreach(List<Tile> path in paths) {
			if(TileInPath(path, targetTile)) {
				foreach(Tile tile in path) {
					newPath.Add(tile);
					if(tile == targetTile) {
						return newPath;
					}
				}
			}
		}
		return newPath;
	}

	private bool TileInPath(List<Tile> path, Tile targetTile) {
		foreach(Tile tile in path) {
			if(targetTile == tile) {
				return true;
			}
		}
		return false;
	}
	
	private IEnumerator ReverseToPath(List<Tile> path) {
		while(!TileInPath(path, playerScript.CurrentTile)) {
			Move lastMove = moveList.LastMove();
			playerScript.MoveTo(lastMove.tile);

			while(playerScript.moving) {
				yield return null;
			}

			ReverseMoveChanges(lastMove);
			moveList.GoBack();
		}


		foreach(Tile nextTile in moveList.Queue) {
			playerScript.MoveTo(nextTile);
			
			while(playerScript.moving) {
				yield return null;
			}
			PassTile(nextTile);
			moveList.Next();
		}
		FinishTurn();
	}

	private IEnumerator ProcessMoveQueue() {
		List<Tile> queue = new List<Tile>();
		queue.AddRange(moveList.Queue);
		foreach(Tile nextTile in queue) {
			playerScript.MoveTo(nextTile);
			
			while(playerScript.moving) {
				yield return null;
			}
			PassTile(nextTile);
			moveList.Next();
		}
		FinishTurn();
	}

	private void PassTile(Tile nextTile) {

	}

	private void FinishTurn() {

	}

	private void ReverseMoveChanges(Move move) {
		playerScript.Cash -= move.cash;
		playerScript.Level -= move.level;
	}

	private Tile GetTileAt(Vector3 mousePosition) {
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			string name = hit.collider.gameObject.name;
			if(name.Substring(name.Length - 11, 4) == "Tile") {
				return hit.collider.gameObject.GetComponent<Tile>();
			}
		}
		return null;
	}
	
	public override IEnumerator Starting() {
		Game script = this.GameLogic.GetComponent<Game>();
		BoardInfo info = script.BoardInfo;
		BuildLevel(info);
		board.BuildPaths();
		AddPlayers(Config.Instance.playerInfo);
		SwitchPlayers(1);
		Camera.main.GetComponent<MoveCamera>().Character = currentPlayer;
		ActionMenu = GameObject.Find("ActionMenu");
		ActionMenu.SetActive(true);
		Dice = GameObject.Find("Dice");
		Dice.renderer.enabled = false;
		DiceSpin.DiceSpun += new DiceSpin.DiceSpinHandler(this.DiceSpun);
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
			Player playerScript = (Player)player.GetComponent<Player>();
			playerScript.CurrentTile = board.bank.GetComponent<Bank>();
			playerScript.PlayerName = entry.Value.Name;
			playerScript.Color = entry.Value.Color;
			playerScript.Hide();
			players.Add(entry.Key, player);
		}
	}

	private void SwitchPlayers(int playerNum) {
		if(currentPlayer != null) {
			currentPlayer.GetComponent<Player>().Hide();
		}
		currentPlayer = players[playerNum];
		playerScript = (Player)currentPlayer.GetComponent<Player>();
		playerScript.Show();
		Camera.main.GetComponent<MoveCamera>().Character = currentPlayer;
	}
}