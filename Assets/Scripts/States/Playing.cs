using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playing : BaseGameState {
	private enum States { ChooseAction, Roll, Move, Confirm };
	private States currentState = States.Move;
	private ICollection<Constants.InputEvents> inputEvents = new List<Constants.InputEvents>() {
		Constants.InputEvents.MouseUp
	};
	private Board board = new Board();
	private Dictionary<int, Player> players;
	private Player currentPlayer;

	public override ICollection<Constants.InputEvents> InputEvents {
		get { return inputEvents; }
	}

	public override void MouseUp(InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		typeof(Playing).GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click").Invoke(this, new object[] { e });
	}
	
	public void Move_LeftClick(InputEventArgs e) {
		Tile tile = (Tile)GetTileAt(e.MousePosition).GetComponent<Tile>();
		if(tile != null && board.ValidMove(tile.BoardX, tile.BoardY, 0, 0)) {

		}
	}

	private GameObject GetTileAt(Vector3 mousePosition) {
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			string name = hit.collider.gameObject.name;
			if(name.Substring(name.Length - 11, 4) == "Tile") {
				return hit.collider.gameObject;
			}
		}
		return null;
	}
	
	public override IEnumerator Start() {
		Game script = this.GameLogic.GetComponent<Game>();
		BoardInfo info = script.BoardInfo;
		BuildLevel(info);
		AddPlayers(Config.Instance.playerInfo);
		yield return null;
	}

	public override IEnumerator End() { yield return null; }
	public override void MouseClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void MouseHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyClick(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyHeld(InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyUp(InputEventArgs e) { throw new System.NotImplementedException (); }

	void BuildLevel(BoardInfo info) {
		for(int i = 0; i < info.Tiles.Count; i++) {
			GameObject newTile = TileFactory.Instance.Build(info.Tiles[i].Code, info.Tiles[i].TileX, info.Tiles[i].TileY, info.Tiles[i].District);
			board.AddTile(newTile, info.Tiles[i].TileX, info.Tiles[i].TileY);
		}
	}

	void AddPlayers(Dictionary<int,PlayerInfo> playerInfo) {
		foreach(KeyValuePair<int, PlayerInfo> entry in playerInfo) {

		}
	}
}