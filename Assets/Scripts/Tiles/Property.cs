using UnityEngine;
using System.Collections;

public class Property : Tile {
	private string district;

	public string District {
		get { return district; }
		set { district = value; }
	}

	public void Awake() {
		Type = Constants.TileCodes.Property;
		transform.FindChild("Sign").FindChild("Cost").GetComponent<TextMesh>().text = Random.Range(100, 500).ToString();
	}

	public void SetDistrict(string district) {
		District = district;
		transform.FindChild("Border").gameObject.GetComponent<Renderer>().material.color = Config.Instance.GetDistrictColor(district);
	}
}