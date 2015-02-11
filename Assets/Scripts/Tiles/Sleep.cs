using UnityEngine;
using System.Collections;

public class Sleep : Tile {
	public Sleep() {
		Type = Constants.TileCodes.Sleep;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/SleepTile"));
	}
}