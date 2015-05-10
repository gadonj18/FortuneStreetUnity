using UnityEngine;
using System.Collections;

public class Property : Tile {
	private Transform baseModel;
	private Transform borderModel;
	private Transform costText;
	private Transform houseModel;
	private Transform signModel;
	private Transform signText;

	public Material OwnedMaterial;
	public Material UnownedMaterial;

	private PlayerController owner;
	public PlayerController Owner {
		get { return owner; }
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

	public void Awake() {
		Type = Constants.TileCodes.Property;
		baseModel = transform.FindChild("Base");
		borderModel = transform.FindChild("Border");
		costText = transform.FindChild("Cost");
		houseModel = transform.FindChild("House");
		signModel = transform.FindChild("Sign");
		signText = transform.FindChild("Sign/Cost");

		baseModel.GetComponent<Renderer>().material = UnownedMaterial;
		costText.GetComponent<Renderer>().enabled = false;
		houseModel.GetComponent<Renderer>().enabled = false;
		signModel.GetComponent<Renderer>().enabled = true;
		signText.GetComponent<Renderer>().enabled = true;
	}

	public void Start() {
		costText.GetComponent<TextMesh>().text = ShopPrice.ToString();
		signText.GetComponent<TextMesh>().text = ShopValue.ToString();
	}

	public void SetDistrict(string district) {
		District = district;
		borderModel.GetComponent<Renderer>().material.color = Config.Instance.GetDistrictColor(district);
	}

	public void ChangeOwner(PlayerController newOwner) {
		if(owner == null) {
			baseModel.GetComponent<Renderer>().material = OwnedMaterial;
			costText.GetComponent<Renderer>().enabled = true;
			houseModel.GetComponent<Renderer>().enabled = true;
			signModel.GetComponent<Renderer>().enabled = false;
			signText.GetComponent<Renderer>().enabled = false;
		}
		owner = newOwner;
		baseModel.GetComponent<Renderer>().material.color = newOwner.Color;
	}
}