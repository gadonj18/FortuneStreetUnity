using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyActions : BaseTileActions {
	private Property TileScript;

	private void Start() {
		TileScript = Tile.GetComponent<Property>();
	}

	public override IEnumerator PassTile(bool reversing = false) {
		GameState.UnownedPropertyInfo.transform.Find("District").Find("Text").GetComponent<Text>().text = "District " + TileScript.District;
		GameState.UnownedPropertyInfo.transform.Find("ShopValueBG").Find("ShopValue").GetComponent<Text>().text = "$" + TileScript.ShopValue;
		GameState.UnownedPropertyInfo.transform.Find("ShopPriceBG").Find("ShopPrice").GetComponent<Text>().text = "$" + TileScript.ShopPrice;
		GameState.UnownedPropertyInfo.transform.Find("Name").GetComponent<Text>().text = TileScript.PropertyName;
		GameState.UnownedPropertyInfo.SetActive(true);
		yield return null;
	}
	
	public override IEnumerator LandOnTile() {
		if(TileScript.Owner == null) {

		} else {

		}
		yield return null;
	}
	
	public override IEnumerator LeaveTile() {
		GameState.UnownedPropertyInfo.SetActive(false);
		yield return null;
	}
}