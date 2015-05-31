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
	private Vector3 origScale = new Vector3(0.5f, 0.5f, 0.5f);
	private Vector3 smallScale = new Vector3(0.1f, 0.1f, 0.1f);
	private List<Vector3> tileCornerOffsets = new List<Vector3>() {
		new Vector3(-0.5857f, 0f, -0.129f), //Front left
		new Vector3(0.5857f, 0f, -0.129f), //Front right
		new Vector3(-0.5857f, 0f, 1.011f), //Back left
		new Vector3(0.5857f, 0f, 1.011f) //Back right
	};
	private bool hidden = false;

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

	public int NumProperties {
		get {
			int num = 0;
			foreach(KeyValuePair<string, List<Property>> district in Properties) {
				num += district.Value.Count;
			}
			return num;
		}
	}

	public int NextSalary {
		get {
			return 500 + ((level + 1) * 50) + (NumProperties * 25);
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
		if(gameLogic.CurrentState != Playing.States.Init) {
			ScoreUI.transform.FindChild("Name").GetComponent<Text>().text = PlayerName;
			ScoreUI.transform.FindChild("Cash").GetComponent<Text>().text = "$" + Cash.ToString();
			ScoreUI.transform.FindChild("Worth").GetComponent<Text>().text = "$" + Worth.ToString();
			if(Cash < 0) {
				ScoreUI.transform.Find("Cash").GetComponent<Text>().color = Color.red;
			}
			if(suits[Constants.Suits.Club]) {
				ScoreUI.transform.FindChild("Suits/Club").GetComponent<Image>().color = Constants.SuitColors[Constants.Suits.Club];
			}
			if(suits[Constants.Suits.Heart]) {
				ScoreUI.transform.FindChild("Suits/Heart").GetComponent<Image>().color = Constants.SuitColors[Constants.Suits.Heart];
			}
			if(suits[Constants.Suits.Spade]) {
				ScoreUI.transform.FindChild("Suits/Spade").GetComponent<Image>().color = Constants.SuitColors[Constants.Suits.Spade];
			}
			if(suits[Constants.Suits.Diamond]) {
				ScoreUI.transform.FindChild("Suits/Diamond").GetComponent<Image>().color = Constants.SuitColors[Constants.Suits.Diamond];
			}
		}
	}

	public int NumPropertiesInDistrict(string district) {
		return Properties[district].Count;
	}

	public bool CanLevelUp() {
		int numSuits = 0;
		if(suits[Constants.Suits.Wild]) numSuits++;
		if(suits[Constants.Suits.Club]) numSuits++;
		if(suits[Constants.Suits.Heart]) numSuits++;
		if(suits[Constants.Suits.Spade]) numSuits++;
		if(suits[Constants.Suits.Diamond]) numSuits++;
		if(numSuits >= 4) {
			return true;
		}
		return false;
	}

	public void LevelUp() {
		int numSuits = 0;
		if(suits[Constants.Suits.Club]) {
			suits[Constants.Suits.Club] = false;
			numSuits++;
		}
		if(suits[Constants.Suits.Heart]) {
			suits[Constants.Suits.Heart] = false;
			numSuits++;
		}
		if(suits[Constants.Suits.Spade]) {
			suits[Constants.Suits.Spade] = false;
			numSuits++;
		}
		if(suits[Constants.Suits.Diamond]) {
			suits[Constants.Suits.Diamond] = false;
			numSuits++;
		}
		if(numSuits < 4 && suits[Constants.Suits.Wild]) {
			suits[Constants.Suits.Wild] = false;
		}

		cash += NextSalary;
		level++;
	}

	public IEnumerator Hide() {
		while(hidden) {
			yield return null;
		}
		Vector3 startPos = transform.position;
		Vector3 tilePos = new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z - 0.491f) + tileCornerOffsets[currentTile.PlayersOnTile.Count - 1];
		float t = 0f;
		float totalTime = 0.5f;
		while(t < totalTime) {
			transform.localScale = Vector3.Lerp(origScale, smallScale, t / totalTime);
			transform.position = Vector3.Lerp(startPos, tilePos, t / totalTime);
			t += Time.deltaTime;
			yield return null;
		}
		hidden = true;
		yield break;
	}

	public IEnumerator Show() {
		while(!hidden) {
			yield return null;
		}
		Vector3 startPos = transform.position;
		Vector3 tilePos = new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z - 0.491f);
		float t = 0f;
		float totalTime = 0.5f;
		while(t < totalTime) {
			transform.localScale = Vector3.Lerp(smallScale, origScale, t / totalTime);
			transform.position = Vector3.Lerp(startPos, tilePos, t / totalTime);
			t += Time.deltaTime;
			yield return null;
		}
		hidden = false;
		yield break;
	}

	public IEnumerator MoveToTile(Tile targetTile) {
		direction = gameLogic.Board.GetDir(currentTile, targetTile);
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

	public void AddProperty(Property property) {
		if(!properties.ContainsKey(property.District)) properties[property.District] = new List<Property>();
		properties[property.District].Add(property);
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