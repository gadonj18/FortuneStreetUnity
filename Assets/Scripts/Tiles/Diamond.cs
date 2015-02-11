using UnityEngine;
using System.Collections;

public class Diamond : Tile {
	public Diamond() {
		Type = Constants.TileCodes.Diamond;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DiamondTile"));
	}
}