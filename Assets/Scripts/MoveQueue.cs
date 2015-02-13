using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveList {
	private List<Tile> tiles = new List<Tile>();
	private Tile currentTile;
	private int currentIdx;

	public MoveList(Tile initialTile) {
		tiles.Add(initialTile);
		currentTile = initialTile;
		currentIdx = 0;
	}

	public void GoBack() {
		tiles.RemoveRange(currentIdx, tiles.Count - currentIdx);
		currentIdx--;
	}

	public void ClearQueue() {
		tiles.RemoveRange(currentIdx + 1, tiles.Count - currentIdx);
	}
}