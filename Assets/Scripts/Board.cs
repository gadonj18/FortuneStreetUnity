using UnityEngine;
using System.Collections.Generic;

public class Board {
	private Dictionary<int, Dictionary<int, GameObject>> tiles;
	public GameObject bank;

	public Board() {
		tiles = new Dictionary<int, Dictionary<int, GameObject>>();
		bank = null;
	}
	
	public void AddTile(GameObject tile, int x, int y) {
		if(!tiles.ContainsKey (x)) {
			tiles.Add(x, new Dictionary<int, GameObject> ());
		}
		if(!tiles[x].ContainsKey(y)) {
			tiles[x].Add(y, tile);
		} else {
			tiles[x][y] = tile;
		}
		if (tile.GetComponent<Tile>().Type == Constants.TileCodes.Bank) {
			bank = tile;
		}
	}

	public void AddTile(GameObject tile, Vector2 pos) {
		AddTile(tile, (int)pos.x, (int)pos.y);
	}
	
	public GameObject GetTile(int x, int y) {
		return tiles[x][y];
	}

	public GameObject GetTile(Vector2 pos) {
		return GetTile((int)pos.x, (int)pos.y);
	}
	
	public bool ValidMove(int x1, int y1, int x2, int y2) {
		int diffX = System.Math.Abs(x2 - x1);
		int diffY = System.Math.Abs(y2 - y1);
		return (diffX <= 2 && diffY <= 2);
	}
	
	public List<List<Tile>> GetPaths(Tile originTile, Constants.Directions dir, int diceRoll) {
		List<List<Tile>> paths = new List<List<Tile>>();
		return paths;
	}
}