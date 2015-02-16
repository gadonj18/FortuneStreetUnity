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
	private int movesLeft = 0;
	private GameObject ActionMenu;
	private GameObject Dice;

	public override ICollection<Constants.InputEvents> InputEvents {
		get { return inputEvents; }
	}

	public Playing() {
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
		GameObject dice = GameObject.Find("Dice");
		dice.renderer.enabled = true;
		dice.GetComponent<DiceSpin>().Roll(movesLeft);
		currentState = States.Move;
	}

	public void DiceSpun() {
		Dice.renderer.enabled = false;
		Player player = (Player)currentPlayer.GetComponent<Player>();
		List<List<Tile>> paths = board.GetPaths(player.CurrentTile, player.Direction, movesLeft);
		Debug.Log(paths.Count);
	}

	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		System.Reflection.MethodInfo method = typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click");
		if(method != null) method.Invoke(this, new object[] { e });
	}
	
	public void Move_LeftClick(InputEventArgs e) {
		if(!currentPlayer.GetComponent<Player>().moving) {
			Tile targetTile = GetTileAt(e.MousePosition);
			if(targetTile != null) {
				Tile currentTile = currentPlayer.GetComponent<Player>().CurrentTile;
				//List<List<Tile>> paths = board.GetPaths(currentTile, currentPlayer.GetComponent<Player>().dir, 6);
				//if(path.Count > 0/* && path.Count <= diceRoll*/) {

				//	currentPlayer.GetComponent<Player>().MoveTo(targetTile);
				//}
			}
		}
	}

	private void PlayerMove(PlayerMoveEventArgs e) {
		Debug.Log("Moved");
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
	
	public override IEnumerator Start() {
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
		yield return null;
	}

	public override IEnumerator End() { yield return null; }
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
		Player.PlayerMove += new Player.PlayerMoveHandler(this.PlayerMove);
	}

	private void SwitchPlayers(int playerNum) {
		if(currentPlayer != null) {
			currentPlayer.GetComponent<Player>().Hide();
		}
		currentPlayer = players[playerNum];
		currentPlayer.GetComponent<Player>().Show();
		Camera.main.GetComponent<MoveCamera>().Character = currentPlayer;
	}
}