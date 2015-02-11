using UnityEngine;
using System.Collections;

public class Dice : Tile {
	public Dice() {
		Type = Constants.TileCodes.Dice;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/CommissionTile"));
	}
}