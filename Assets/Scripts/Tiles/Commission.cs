using UnityEngine;
using System.Collections;

public class Commission : Tile {
	public Commission() {
		Type = Constants.TileCodes.Commission;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/CommissionTile"));
	}
}