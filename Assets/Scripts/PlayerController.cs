using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public Playing gameLogic;
	private string playerName;
	private Color color;
	private Tile currentTile;
	private Constants.Directions direction = Constants.Directions.Any;
	private int cash;
	private Dictionary<string, List<Property>> properties;
	private Dictionary<string, int> stocks;
	private Dictionary<Constants.Suits, bool> suits;
	private int level;
	private int hopLeftHash = Animator.StringToHash("Base Layer.HopLeft");
	private int hopRightHash = Animator.StringToHash("Base Layer.HopRight");
	private bool leftHopNext = true;
	public bool moving = false;
	public float MoveSpeed = 2.6f;
	public GameObject ScoreUI;

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

	public Dictionary<Constants.Suits, bool> Suits {
		get { return suits; }
	}

	public int Level {
		get { return level; }
		set { level = value; }
	}

	public int Worth {
		get {
			int worth = 0;
			worth += cash;
			worth += PropertyWorth;
			worth += StockWorth;
			return worth;
		}
	}

	public int PropertyWorth {
		get {
			int worth = 0;
			foreach(KeyValuePair<string, List<Property>> district in Properties) {
				foreach(Property property in district.Value) {
					worth += property.TotalValue;
				}
			}
			return worth;
		}
	}

	public int StockWorth {
		get {
			int worth = 0;
			foreach(KeyValuePair<string, int> district in Stocks) {
				worth += district.Value * gameLogic.StockPrices[district.Key];
			}
			return worth;
		}
	}

	public void Awake() {
		properties = new Dictionary<string, List<Property>>();
		stocks = new Dictionary<string, int>();
		suits = new Dictionary<Constants.Suits, bool> ();
		Constants.Suits[] cardList = (Constants.Suits[])System.Enum.GetValues(typeof(Constants.Suits));
		foreach(Constants.Suits card in cardList) {
			suits[card] = false;
		}
		transform.rotation = Quaternion.Euler(-15f, 180f, 0f);
	}

	public void Update() {
		ScoreUI.transform.Find("Name").GetComponent<Text>().text = PlayerName;
		ScoreUI.transform.Find("Cash").GetComponent<Text>().text = "$" + Cash.ToString();
		ScoreUI.transform.Find("Worth").GetComponent<Text>().text = "$" + Cash.ToString();
		string cardString = "";
		if(suits[Constants.Suits.Club]) cardString += "C";
		if(suits[Constants.Suits.Heart]) cardString += "H";
		if(suits[Constants.Suits.Spade]) cardString += "S";
		if(suits[Constants.Suits.Diamond]) cardString += "D";
		ScoreUI.transform.Find("Cards").GetComponent<Text>().text = cardString;
	}

	public int NumPropertiesInDistrict(string district) {
		return Properties[district].Count;
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