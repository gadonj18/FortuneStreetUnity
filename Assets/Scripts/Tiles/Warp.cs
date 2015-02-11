using UnityEngine;
using System.Collections;

public class Warp : Tile {
	public Warp() {
		Type = Constants.TileCodes.Warp;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/WarpTile"));
	}
}