using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Tile : MonoBehaviour {
	private int boardX;
	private int boardY;
	private Constants.TileCodes type;
	private List<Constants.Directions> dirs = new List<Constants.Directions>();

	private List<PlayerController> playersOnTile = new List<PlayerController>();
	public List<PlayerController> PlayersOnTile {
		get { return playersOnTile; }
	}

	public int BoardX {
		get { return boardX; }
		set { boardX = value; }
	}

	public int BoardY {
		get { return boardY; }
		set { boardY = value; }
	}

	public Constants.TileCodes Type {
		get { return type; }
		set { type = value; }
	}

	public List<Constants.Directions> Dirs {
		get { return dirs; }
		set {
			dirs.Clear();
			dirs.AddRange(value);
		}
	}

	public Vector2 GetBoardPos() {
		return new Vector2(BoardX, BoardY);
	}
}