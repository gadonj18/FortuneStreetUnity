using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	private string playerName;
	private Color color;
	private Tile lastTile;
	private Tile currentTile;
	private Tile nextTile;
	private int cash;
	private Dictionary<string, List<Property>> properties;
	private Dictionary<string, int> stocks;
	private Dictionary<Constants.Cards, bool> cards;
	private int level;
	private int hopLeftHash = Animator.StringToHash("Base Layer.HopLeft");
	private int hopRightHash = Animator.StringToHash("Base Layer.HopRight");
	private bool leftHopNext = true;
	public float MoveSpeed = 2.6f;

	public delegate void PlayerMoveHandler(PlayerMoveEventArgs e);
	public static event PlayerMoveHandler PlayerMove;

	public string PlayerName {
		get { return playerName; }
		set { playerName = value; }
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
		StartCoroutine("MoveToTarget");
	}

	private IEnumerator MoveToTarget() {
		Vector3 targetPos = new Vector3(nextTile.transform.position.x, transform.position.y, nextTile.transform.position.z - 0.491f);
		GetComponent<Animator>().Play(leftHopNext ? hopLeftHash : hopRightHash);
		leftHopNext = !leftHopNext;

		transform.rotation = Quaternion.LookRotation(targetPos - transform.position) * Quaternion.Euler(-15f, 0f, 0f);
		while(transform.position != targetPos) {
			transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * MoveSpeed);
			yield return null;
		}
		if(PlayerMove != null) {
			PlayerMove(new PlayerMoveEventArgs(nextTile));
		}
		lastTile = currentTile;
		currentTile = nextTile;
		nextTile = null;
		yield return null;
	}

	void OnHopEnd() {
		transform.rotation = Quaternion.Euler(-15f, 180f, 0f);
	}
}	

public class PlayerMoveEventArgs : System.EventArgs {
	public Tile tile;

	public PlayerMoveEventArgs(Tile targetTile) {
		tile = targetTile;
	}
}