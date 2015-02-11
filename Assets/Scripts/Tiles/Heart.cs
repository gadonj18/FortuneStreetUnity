using UnityEngine;
using System.Collections;

public class Heart : Tile {
	public Heart() {
		Type = Constants.TileCodes.Heart;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeartTile"));
	}
}