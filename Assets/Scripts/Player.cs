using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
	private GameObject gameObject;
	private string name;
	private Color color;
	private Tile lastTile;
	private Tile currentTile;
	private Tile nextTile;
	private int cash;
	private Dictionary<string, List<Property>> properties;
	private Dictionary<string, int> stocks;
	private Dictionary<Constants.Cards, bool> cards;
	private int level;

	public string Name {
		get { return name; }
		set { name = value; }
	}

	public Color Color {
		get { return color; }
		set { color = value; }
	}

	public Tile LastTile {
		get { return lastTile; }
		set { lastTile = value; }
	}

	public Tile CurrentTile {
		get { return currentTile; }
		set { currentTile = value; }
	}

	public Tile NextTile {
		get { return nextTile; }
		set { nextTile = value; }
	}

	public int Cash {
		get { return cash; }
		set { cash = value; }
	}

	public Dictionary<string, List<Property>> Properties {
		get { return properties; }
	}

	public Dictionary<string, int> Stocks {
		get { return stocks; }
	}

	public Dictionary<Constants.Cards, bool> Cards {
		get { return cards; }
	}

	public int Level {
		get { return level; }
		set { level = value; }
	}

	public Player() {
		properties = new Dictionary<string, List<Property>>();
		stocks = new Dictionary<string, int>();
		cards = new Dictionary<Constants.Cards, bool> ();
	}
}	