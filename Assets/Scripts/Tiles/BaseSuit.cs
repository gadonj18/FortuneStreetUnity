using UnityEngine;
using System.Collections;

public abstract class BaseSuit : Tile {
	private Constants.Suits suit;
	public Constants.Suits Suit {
		get { return suit; }
		set { suit = value; }
	}
}