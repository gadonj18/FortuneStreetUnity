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
		dice.GetComponent<Renderer>().enabled = true;
		dice.GetComponent<DiceSpin>().Roll(movesLeft);
	}

	public void DiceSpun() {
		Dice.GetComponent<Renderer>().enabled = false;
		paths = board.GetPaths(playerScript.CurrentTile, playerScript.Direction, movesLeft);
		currentState = States.Move;
	}

	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		System.Reflection.MethodInfo method = typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click");
		if(method != null) method.Invoke(this, new object[] { e });
	}
	
	public void Move_LeftClick(InputEventArgs e) {
		Tile targetTile = GetTileAt(e.MousePosition);
		if(targetTile != null && targetTile != playerScript.CurrentTile) {
			StartCoroutine("MoveToTile", targetTile);
		}
	}

	private IEnumerator MoveToTile(Tile targetTile) {
		List<Tile> path = GetPathTo(targetTile);
		if(path.Count > 0) {
			while(playerScript.moving) {
				yield return null;
			}
			StopCoroutine("ReverseToPath");
			StopCoroutine("ReverseToTile");
			StopCoroutine("ProcessMoveQueue");

			//Tile you're on is NOT in the new path
			if(moveList.Moves.Count > 0 && !TileInPath(path, playerScript.CurrentTile)) {
				//Reverse to intersect with the new path
				yield return StartCoroutine("ReverseToPath", path);
			}

			//Reverse in current path if already past target
			if(path.Count < moveList.Moves.Count) {
				yield return StartCoroutine("ReverseToTile", targetTile);
			} else {
				//Remove any already moved to tiles from the new path
				for(int i = 0; i < moveList.Moves.Count; i++) {
					if(moveList.Moves[i].tile == playerScript.CurrentTile) {
						break;
					}
					path.RemoveAt(0);
				}

				//Add the remainder of the path to the queue
				moveList.AddQueue(path);

				//And move to the queue
				StartCoroutine("ProcessMoveQueue");
			}
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
		for(int i = moveList.Moves.Count; i >= 0; i--) {
			if(TileInPath(path, playerScript.CurrentTile)) {
				break;
			}

			Move lastMove = moveList.Moves[i];
			playerScript.MoveTo(lastMove.tile);

			while(playerScript.moving) {
				yield return null;
			}
			
			ReverseMoveChanges(lastMove);
			moveList.GoBack();
		}
	}

	private IEnumerator ReverseToTile(Tile target) {
		for(int i = moveList.Moves.Count - 1; i >= 0; i--) {
			if(moveList.Moves[i].tile == target) {
				break;
			}

			Move lastMove = moveList.Moves[i];
			playerScript.MoveTo(lastMove.tile);
			
			while(playerScript.moving) {
				yield return null;
			}
			
			ReverseMoveChanges(lastMove);
			moveList.GoBack();
		}
	}
	
	private void ReverseMoveChanges(Move move) {
		playerScript.Cash -= move.cash;
		playerScript.Level -= move.level;
		//foreach(KeyValuePair<Constants.Cards, bool> cardChange in move.cards) {

		//}
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
		Dice.GetComponent<Renderer>().enabled = false;
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