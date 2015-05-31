using UnityEngine;
using System.Collections.Generic;

public class Board {
	private Dictionary<int, Dictionary<int, GameObject>> tiles;
	public Dictionary<int, Dictionary<int, GameObject>> Tiles {
		get { return tiles; }
	}
	private int maxX;
	public int MaxX {
		get { return maxX; }
		set { maxX = value; }
	}
	private int maxY;
	public int MaxY {
		get { return maxY; }
		set { maxY = value; }
	}
	public GameObject bank;
	public List<string> Districts;
	public Dictionary<int, Dictionary<int, Constants.Directions>> XYtoDir = new Dictionary<int, Dictionary<int, Constants.Directions>>() {
		{ -2, new Dictionary<int, Constants.Directions>() {
				{ -2, Constants.Directions.SW },
				{ -1, Constants.Directions.WSW },
				{ 0, Constants.Directions.W },
				{ 1, Constants.Directions.WNW },
				{ 2, Constants.Directions.NW },
			}
		},
		{ -1, new Dictionary<int, Constants.Directions>() {
				{ -2, Constants.Directions.SSW },
				{ 2, Constants.Directions.NNW },
			}
		},
		{ 0, new Dictionary<int, Constants.Directions>() {
				{ -2, Constants.Directions.S },
				{ 2, Constants.Directions.N },
			}
		},
		{ 1, new Dictionary<int, Constants.Directions>() {
				{ -2, Constants.Directions.SSE },
				{ 2, Constants.Directions.NNE },
			}
		},
		{ 2, new Dictionary<int, Constants.Directions>() {
				{ -2, Constants.Directions.SE },
				{ -1, Constants.Directions.ESE },
				{ 0, Constants.Directions.E },
				{ 1, Constants.Directions.ENE },
				{ 2, Constants.Directions.NE },
			}
		}
	};

	public Dictionary<Constants.Directions, Vector2> DirToXY = new Dictionary<Constants.Directions, Vector2>() {
		{ Constants.Directions.SW, new Vector2(-2, -2) },
		{ Constants.Directions.WSW, new Vector2(-2, -1) },
		{ Constants.Directions.W, new Vector2(-2, 0) },
		{ Constants.Directions.WNW, new Vector2(-2, 1) },
		{ Constants.Directions.NW, new Vector2(-2, 2) },
		{ Constants.Directions.SSW, new Vector2(-1, -2) },
		{ Constants.Directions.NNW, new Vector2(-1, 2) },
		{ Constants.Directions.S, new Vector2(0, -2) },
		{ Constants.Directions.N, new Vector2(0, 2) },
		{ Constants.Directions.SSE, new Vector2(1, -2) },
		{ Constants.Directions.NNE, new Vector2(1, 2) },
		{ Constants.Directions.SE, new Vector2(2, -2) },
		{ Constants.Directions.ESE, new Vector2(2, -1) },
		{ Constants.Directions.E, new Vector2(2, 0) },
		{ Constants.Directions.ENE, new Vector2(2, 1) },
		{ Constants.Directions.NE, new Vector2(2, 2) }
	};

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
		Tile tileScript = (Tile)tile.GetComponent<Tile>();
		if(tileScript.Type == Constants.TileCodes.Bank) {
			bank = tile;
		} else if(tileScript.Type == Constants.TileCodes.Property) {
			//Districts.Add(((Property)tileScript).District);
		}
	}

	public void BuildPaths() {
		foreach(KeyValuePair<int, Dictionary<int, GameObject>> column in tiles) {
			foreach(KeyValuePair<int, GameObject> row in column.Value) {
				row.Value.GetComponent<Tile>().Dirs = GetPossibleDirections(column.Key, row.Key);
			}
		}
	}

	public List<Constants.Directions> GetPossibleDirections(int tileX, int tileY) {
		List<Constants.Directions> dirs = new List<Constants.Directions>();
		for(int x = -2; x <= 2; x++) {
			for(int y = -2; y <= 2; y++) {
				if(x != 0 || y != 0) {
					if(tiles.ContainsKey(x + tileX) && tiles[x + tileX].ContainsKey(y + tileY)) {
						dirs.Add(XYtoDir[x][y]);
					}
				}
			}
		}
		return dirs;
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
	
	public List<List<Tile>> GetPaths(Tile tile, Constants.Directions direction, int movesLeft) {
		List<List<Tile>> newPaths = new List<List<Tile>>();
		foreach(Constants.Directions dir in tile.Dirs) {
			//if(direction == Constants.Directions.Any || dir != ReverseDirection(direction)) {
			if(direction == Constants.Directions.Any || dir != ReverseDirection(direction)) {
				Vector2 newDir = DirToXY[dir];
				List<Tile> newPath = new List<Tile>();
				Tile newTile = (Tile)tiles[(int)newDir.x + tile.BoardX][(int)newDir.y + tile.BoardY].GetComponent<Tile>();
				newPath.Add(newTile);

				if(movesLeft > 0) {
					List<List<Tile>> subPaths = GetPaths(newTile, XYtoDir[(int)newDir.x * -1][(int)newDir.y * -1], movesLeft - 1);
					foreach(List<Tile> path in subPaths) {
						path.Insert(0, newTile);
						newPaths.Add(path);
					}
				} else {
					newPaths.Add(newPath);
				}
			}
		}
		return newPaths;
	}

	public Tile GetTileAt(Vector3 mousePosition) {
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

	public List<Tile> GetPathToTile(List<List<Tile>> paths, Tile targetTile) {
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

	public bool TileInPath(List<Tile> path, Tile targetTile) {
		foreach(Tile tile in path) {
			if(targetTile == tile) {
				return true;
			}
		}
		return false;
	}

	public Constants.Directions GetDir(Tile from, Tile to) {
		int diffX = to.BoardX - from.BoardX;
		int diffY = to.BoardY - from.BoardY;
		return XYtoDir[diffX][diffY];
	}

	public Constants.Directions ReverseDirection(Constants.Directions dir) {
		return XYtoDir[(int)(DirToXY[dir].x) * -1][(int)(DirToXY[dir].y) * -1];
	}
}