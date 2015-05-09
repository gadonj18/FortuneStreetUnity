using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyActions : BaseTileActions {
	private Property TileScript;

	private void Start() {
		TileScript = Tile.GetComponent<Property>();
	}

	public override IEnumerator PassTile(bool reversing = false) {
		UIManager.Instance.UnownedPropertyInfo.transform.Find("District").Find("Text").GetComponent<Text>().text = "District " + TileScript.District;
		UIManager.Instance.UnownedPropertyInfo.transform.Find("ShopValueBG").Find("ShopValue").GetComponent<Text>().text = "$" + TileScript.ShopValue;
		UIManager.Instance.UnownedPropertyInfo.transform.Find("ShopPriceBG").Find("ShopPrice").GetComponent<Text>().text = "$" + TileScript.ShopPrice;
		UIManager.Instance.UnownedPropertyInfo.transform.Find("Name").GetComponent<Text>().text = TileScript.PropertyName;
		UIManager.Instance.UnownedPropertyInfo.SetActive(true);
		yield return null;
	}
	
	public override IEnumerator LandOnTile() {
		UIManager.YesButtonClick += new UIManager.UIButtonHandler(PurchaseYes_Click);
		UIManager.NoButtonClick += new UIManager.UIButtonHandler(PurchaseNo_Click);
		if(TileScript.Owner == null) {
			UIManager.Instance.YesNoMenu.transform.FindChild("Title/Text").GetComponent<Text>().text = "Purchase Property?";
			UIManager.Instance.YesNoMenu.SetActive(true);
		} else {

		}
		yield return null;
	}
	
	public override IEnumerator LeaveTile() {
		UIManager.Instance.UnownedPropertyInfo.SetActive(false);
		yield return null;
	}
	
	public void PurchaseYes_Click(UIEventArgs e) {
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(PurchaseYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(PurchaseNo_Click);
	}
	
	public void PurchaseNo_Click(UIEventArgs e) {
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(PurchaseYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(PurchaseNo_Click);
	}
}