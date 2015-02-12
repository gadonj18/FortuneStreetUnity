using UnityEngine;
using System.Collections;

public abstract class Tile : MonoBehaviour {
	private int boardX;
	private int boardY;
	private Constants.TileCodes type;

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

	public Vector2 GetBoardPos() {
		return new Vector2(BoardX, BoardY);
	}
}