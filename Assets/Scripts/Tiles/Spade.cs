using UnityEngine;
using System.Collections;

public class Spade : Tile {
	public Spade() {
		Type = Constants.TileCodes.Spade;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/SpadeTile"));
	}
}