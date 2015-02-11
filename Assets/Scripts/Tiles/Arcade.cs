using UnityEngine;
using System.Collections;

public class Arcade : Tile {
	public Arcade() {
		Type = Constants.TileCodes.Arcade;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/CommissionTile"));
	}
}