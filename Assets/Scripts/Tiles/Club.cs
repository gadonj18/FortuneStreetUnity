using UnityEngine;
using System.Collections;

public class Club : Tile {
	public Club() {
		Type = Constants.TileCodes.Club;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/ClubTile"));
	}
}