using UnityEngine;
using System.Collections;

public class Property : Tile {
	private PlayerController owner;
	public PlayerController Owner {
		get { return owner; }
		set { owner = value; }
	}
	private string propertyName;
	public string PropertyName {
		get { return propertyName; }
		set { propertyName = value; }
	}
	private int shopValue;
	public int ShopValue {
		get { return shopValue; }
		set { shopValue = value; }
	}
	private int investedAmount;
	public int InvestedAmount {
		get { return investedAmount; }
		set { investedAmount = value; }
	}
	private string district;
	public string District {
		get { return district; }
		set { district = value; }
	}
	public int ShopPrice {
		get { return Mathf.FloorToInt((shopValue + investedAmount) / 5f * (owner == null ? 1 : owner.NumPropertiesInDistrict(District)) * 0.8f); }
	}
	public int TotalValue {
		get { return ShopValue + InvestedAmount; }
	}

	public void Start() {
		Type = Constants.TileCodes.Property;
		transform.FindChild("Sign").FindChild("Cost").GetComponent<TextMesh>().text = ShopValue.ToString();
	}

	public void SetDistrict(string district) {
		District = district;
		transform.FindChild("Border").gameObject.GetComponent<Renderer>().material.color = Config.Instance.GetDistrictColor(district);
	}
}