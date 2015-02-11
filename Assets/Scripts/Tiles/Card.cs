using UnityEngine;
using System.Collections;

public class Card : Tile {
	public Card() {
		Type = Constants.TileCodes.Card;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/CardTile"));
	}
}