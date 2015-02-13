using UnityEngine;
using System.Collections;

public class Property : Tile {
	public void Awake() {
		Type = Constants.TileCodes.Property;
		transform.FindChild("Sign").FindChild("Cost").GetComponent<TextMesh>().text = Random.Range(100, 500).ToString();
	}

	public void SetDistrict(string district) {
		transform.FindChild("Border").gameObject.renderer.material.color = Config.Instance.GetDistrictColor(district);
	}
}