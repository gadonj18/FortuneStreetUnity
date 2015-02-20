using UnityEngine;
using System.Collections;

public class TileFactory {
	private static TileFactory instance;
	private TileFactory() {}
	public static TileFactory Instance {
		get {
			if (instance == null) {
				instance = new TileFactory();
			}
			return instance;
		}
	}

	public GameObject Build(Constants.TileCodes code, int x, int y, string district = null) {
		Object prefab = Resources.Load("Prefabs/Tiles/" + code.ToString() + "Tile");
		if(prefab != null) {
			GameObject Tile = (GameObject)GameObject.Instantiate(prefab);
			Tile.GetComponent<Tile>().BoardX = x;
			Tile.GetComponent<Tile>().BoardY = y;
			if(code == Constants.TileCodes.Property && district != null) {
				Tile.GetComponent<Property>().SetDistrict(district);
			}
			Tile.transform.position = new Vector3(0.95f * x, 0f, 0.95f * y);
			return Tile;
		}

		return null;
	}
}