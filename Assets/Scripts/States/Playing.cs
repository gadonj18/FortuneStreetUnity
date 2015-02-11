using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playing : BaseGameState {
	private Board board = new Board();
	private enum States { ChooseAction, Roll, Move, Confirm };
	private States currentState = States.Move;
	private ICollection<Constants.InputEvents> inputEvents = new List<Constants.InputEvents>() {
		Constants.InputEvents.MouseUp
	};

	public override ICollection<Constants.InputEvents> InputEvents {
		get { return inputEvents; }
	}

	public override void MouseUp (InputEventArgs e) {
		string[] mouseButton = new string[] { "Left", "Middle", "Right" };
		this.GetType().GetMethod(currentState.ToString() + "_" + mouseButton[e.MouseCode] + "Click").Invoke(this, new object[] { e });
	}
	
	private void Move_LeftClick(InputEventArgs e) {

	}
	
	public override IEnumerator Start() {
		Debug.Log("Test");
		Game script = this.GameLogic.GetComponent<Game>();
		BoardInfo info = script.BoardInfo;
		BuildLevel(info);
		yield return null;
	}
	public override IEnumerator End () { yield return null; }
	public override void MouseClick (InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void MouseHeld (InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyClick (InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyHeld (InputEventArgs e) { throw new System.NotImplementedException (); }
	public override void KeyUp (InputEventArgs e) { throw new System.NotImplementedException (); }

	void BuildLevel(BoardInfo info) {
		for(int i = 0; i < info.Tiles.Count; i++) {
			Tile newTile = TileFactory.Instance.Build(info.Tiles[i].Code, info.Tiles[i].TileX, info.Tiles[i].TileY, info.Tiles[i].District);
			board.AddTile(newTile, info.Tiles[i].TileX, info.Tiles[i].TileY);
		}
	}
}