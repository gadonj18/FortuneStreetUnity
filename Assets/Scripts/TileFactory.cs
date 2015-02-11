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

	public Tile Build(Constants.TileCodes code, int x, int y, string district = null) {
		Tile newTile = (Tile)System.Activator.CreateInstance(System.Type.GetType(code.ToString()));
		if (code == Constants.TileCodes.Property && district != null) {
			((Property)newTile).SetDistrict(district);
		}
		newTile.gameObject.transform.position = new Vector3(0.95f * x, 0f, 0.95f * y);
		return newTile;
	}
}