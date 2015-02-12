using UnityEngine;
using System.Collections;

public class Property : Tile {
	public void Awake() {
		Type = Constants.TileCodes.Property;
	}

	public void SetDistrict(string district) {
		transform.FindChild("Border").gameObject.renderer.material.color = Config.Instance.GetDistrictColor(district);
	}
}