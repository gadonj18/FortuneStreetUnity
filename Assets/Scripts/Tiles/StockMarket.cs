using UnityEngine;
using System.Collections;

public class StockMarket : Tile {
	public StockMarket() {
		Type = Constants.TileCodes.StockMarket;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/StockMarketTile"));
	}
}