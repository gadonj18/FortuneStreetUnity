using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
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

	public void Awake() {
		properties = new Dictionary<string, List<Property>>();
		stocks = new Dictionary<string, int>();
		cards = new Dictionary<Constants.Cards, bool> ();
	}
	
	public void Hide() {
		this.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	public void Show() {
		this.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	public void MoveTo(Tile tile) {
		nextTile = tile;
		StartCoroutine("MoveTo", nextTile);
	}

	private IEnumerator MoveTo() {
		Debug.Log("Move");
		transform.rotation = Quaternion.LookRotation(nextTile.transform.position - transform.position) * Quaternion.Euler(-15f, 0f, 0f);
		while(transform.position != nextTile.transform.position) {
			Debug.Log("Moving");
			transform.position = Vector3.MoveTowards (transform.position, nextTile.transform.position, Time.deltaTime * 2f);
			yield return null;
		}
		Debug.Log("Moved");
		lastTile = currentTile;
		currentTile = nextTile;
		nextTile = null;
	}
}	