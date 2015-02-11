using UnityEngine;
using System.Collections;

public class Property : Tile {
	public Property() {
		Type = Constants.TileCodes.Property;
		gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UnownedProperty"));
	}

	public void SetDistrict(string district) {
		gameObject.transform.FindChild("Border").gameObject.renderer.material.color = Config.Instance.GetDistrictColor(district);
	}
}