using UnityEngine;
using System.Collections.Generic;

public class Board {
	private Dictionary<int, Dictionary<int, Tile>> tiles;
	public Bank bank;

	public Board() {
		tiles = new Dictionary<int, Dictionary<int, Tile>>();
		bank = null;
	}
	
	public void AddTile(Tile tile, int x, int y) {
		if(!tiles.ContainsKey (x)) {
			tiles.Add(x, new Dictionary<int, Tile> ());
		}
		if(!tiles[x].ContainsKey(y)) {
			tiles[x].Add(y, tile);
		} else {
			tiles[x][y] = tile;
		}
		if (tile.Type == Constants.TileCodes.Bank) {
			bank = (Bank)tile;
		}
	}
	
	public Tile GetTile(int x, int y) {
		return tiles[x][y];
	}
}