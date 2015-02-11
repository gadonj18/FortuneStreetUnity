using UnityEngine;
using System.Collections;

public class Bank : Tile {
	public Bank() {
		Type = Constants.TileCodes.Bank;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/BankTile"));
	}
}