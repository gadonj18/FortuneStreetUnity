using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	private string playerName;
	private Color color;
	private Tile currentTile;
	private Constants.Directions direction = Constants.Directions.Any;
	private int cash;
	private Dictionary<string, List<Property>> properties;
	private Dictionary<string, int> stocks;
	private Dictionary<Constants.Cards, bool> cards;
	private int level;
	private int hopLeftHash = Animator.StringToHash("Base Layer.HopLeft");
	private int hopRightHash = Animator.StringToHash("Base Layer.HopRight");
	private bool leftHopNext = true;
	public bool moving = false;
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

	public Tile CurrentTile {
		get { return currentTile; }
		set { currentTile = value; }
	}

	public Constants.Directions Direction {
		get { return direction; }
		set { direction = value; }
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
		Constants.Cards[] cardList = (Constants.Cards[])System.Enum.GetValues(typeof(Constants.Cards));
		foreach(Constants.Cards card in cardList) {
			cards[card] = false;
		}
		transform.rotation = Quaternion.Euler(-15f, 180f, 0f);
	}
	
	public void Hide() {
		this.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().enabled = false;
	}

	public void Show() {
		this.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().enabled = true;
	}

	public IEnumerator MoveToTile(Tile targetTile) {
		moving = true;
		Vector3 targetPos = new Vector3(targetTile.transform.position.x, transform.position.y, targetTile.transform.position.z - 0.491f);
		GetComponent<Animator>().Play(leftHopNext ? hopLeftHash : hopRightHash);
		leftHopNext = !leftHopNext;

		transform.rotation = Quaternion.LookRotation(targetPos - transform.position) * Quaternion.Euler(-15f, 0f, 0f);
		float t = 0f;
		float totalTime = 0.8f;
		Vector3 startPos = transform.position;
		while(t < totalTime) {
			transform.position = Vector3.Lerp(startPos, targetPos, t / totalTime);
			t += Time.deltaTime;
			yield return null;
		}
		currentTile = targetTile;
		moving = false;
		if(PlayerMove != null) {
			PlayerMove(new PlayerMoveEventArgs(targetTile));
		}
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