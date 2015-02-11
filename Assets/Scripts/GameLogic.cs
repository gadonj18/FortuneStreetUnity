using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {
	private Config config = Config.Instance;

	public GameObject OwnedProperty;
	public GameObject UnownedProperty;
	public GameObject BankTile;
	public GameObject DiamondTile;
	public GameObject HeartTile;
	public GameObject SpadeTile;
	public GameObject ClubTile;
	public GameObject CardTile;
	public GameObject SleepTile;
	public GameObject CommissionTile;


	void Start () {
		TextAsset BoardFile = (TextAsset)Resources.Load("Boards/board1", typeof(TextAsset));
		JSONNode BoardInfo = JSON.Parse(BoardFile.text);

		for (int i = 0; i < BoardInfo["tiles"].Count; i++) {
			var tile = BoardInfo["tiles"][i];
			GameObject newTile;
			switch(tile["district"].Value) {
			case "Heart":
				newTile = (GameObject)Instantiate(HeartTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Spade":
				newTile = (GameObject)Instantiate(SpadeTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Diamond":
				newTile = (GameObject)Instantiate(DiamondTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Club":
				newTile = (GameObject)Instantiate(ClubTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Arcade":
			case "Dice":
			case "Warp":
			case "StockMarket":
				break;
			case "Bank":
				newTile = (GameObject)Instantiate(BankTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Sleep":
				newTile = (GameObject)Instantiate(SleepTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Commission":
				newTile = (GameObject)Instantiate(CommissionTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			case "Card":
				newTile = (GameObject)Instantiate(CardTile);
				newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
				break;
			default:
				if(tile["x"].AsInt == 10 && tile["y"].AsInt == 1) {
					newTile = (GameObject)Instantiate(OwnedProperty);
					newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
					newTile.transform.FindChild("Border").gameObject.renderer.material.color = config.GetDistrictColor(tile["district"]);
					newTile.transform.FindChild("Base").gameObject.renderer.material.color = Color.green;
					TextMesh cost = (TextMesh) newTile.transform.FindChild("Cost").GetComponent<TextMesh>();
					cost.text = Random.Range(30, 150).ToString();

				} else {
					newTile = (GameObject)Instantiate(UnownedProperty);
					newTile.transform.position = new Vector3(0.95f * tile["x"].AsInt, 0f, 0.95f * tile["y"].AsInt);
					newTile.transform.FindChild("Border").gameObject.renderer.material.color = config.GetDistrictColor(tile["district"]);
					TextMesh cost = (TextMesh) newTile.transform.FindChild("Sign").FindChild("Cost").GetComponent<TextMesh>();
					cost.text = Random.Range(100, 500).ToString();
				}
				break;
			}
		}
	}
}