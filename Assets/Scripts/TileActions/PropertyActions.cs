﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyActions : BaseTileActions {
	private Property TileScript;

	private void Start() {
		TileScript = Tile.GetComponent<Property>();
	}

	public override IEnumerator MoveToTile(bool reversing = false) {
		UIManager.Instance.UnownedPropertyInfo.transform.FindChild("District/Text").GetComponent<Text>().text = "District " + TileScript.District;
		UIManager.Instance.UnownedPropertyInfo.transform.FindChild("ShopValueBG/ShopValue").GetComponent<Text>().text = "$" + TileScript.ShopValue;
		UIManager.Instance.UnownedPropertyInfo.transform.FindChild("ShopPriceBG/ShopPrice").GetComponent<Text>().text = "$" + TileScript.ShopPrice;
		UIManager.Instance.UnownedPropertyInfo.transform.FindChild("Name").GetComponent<Text>().text = TileScript.PropertyName;
		UIManager.Instance.UnownedPropertyInfo.SetActive(true);
		yield break;
	}
	
	public override IEnumerator LandOnTile() {
		if(TileScript.Owner == null) {
			if(GameState.PlayerScript.Worth < TileScript.ShopValue) {
				UIManager.Instance.Message.transform.FindChild("Text").GetComponent<Text>().text = "You cannot afford this property";
				UIManager.Instance.Message.SetActive(true);
				yield return new WaitForSeconds(3);
				UIManager.Instance.Message.SetActive(false);
				FinishTurn();
				yield break;
			} else if(GameState.PlayerScript.Cash + GameState.PlayerScript.StockWorth < TileScript.ShopValue) {
				UIManager.Instance.YesNoMenu.transform.FindChild("Title/Text").GetComponent<Text>().text = "Purchase Property?\n(Must sell shop to afford)";
				UIManager.Instance.YesNoMenu.SetActive(true);
				UIManager.YesButtonClick += new UIManager.UIButtonHandler(PurchaseYes_Click);
				UIManager.NoButtonClick += new UIManager.UIButtonHandler(PurchaseNo_Click);
			} else {
				UIManager.Instance.YesNoMenu.transform.FindChild("Title/Text").GetComponent<Text>().text = "Purchase Property?";
				UIManager.Instance.YesNoMenu.SetActive(true);
				UIManager.YesButtonClick += new UIManager.UIButtonHandler(PurchaseYes_Click);
				UIManager.NoButtonClick += new UIManager.UIButtonHandler(PurchaseNo_Click);
			}
		} else if(TileScript.Owner == GameState.PlayerScript) {

		} else {
			GameState.PlayerScript.Cash -= TileScript.ShopPrice;
			TileScript.Owner.Cash += TileScript.ShopPrice;
			FinishTurn();
		}
		yield break;
	}
	
	public override IEnumerator LeaveTile() {
		UIManager.Instance.UnownedPropertyInfo.SetActive(false);
		yield break;
	}
	
	public void PurchaseYes_Click(UIEventArgs e) {
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(PurchaseYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(PurchaseNo_Click);
		GameState.PlayerScript.Cash -= TileScript.ShopValue;
		GameState.PlayerScript.AddProperty(TileScript);
		TileScript.ChangeOwner(GameState.PlayerScript);
		GameState.FinishTurn();
	}
	
	public void PurchaseNo_Click(UIEventArgs e) {
		UIManager.Instance.YesNoMenu.SetActive(false);
		UIManager.YesButtonClick -= new UIManager.UIButtonHandler(PurchaseYes_Click);
		UIManager.NoButtonClick -= new UIManager.UIButtonHandler(PurchaseNo_Click);
		FinishTurn();
	}
}