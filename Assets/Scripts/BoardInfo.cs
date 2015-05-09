using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

public class BoardInfo {
	private string id;
	private string title;
	private int startCash;
	private int targetWorth;
	private int minPlayers;
	private int maxPlayers;
	private List<TileInfo> tiles;
	private List<string> districts;

	public string ID {
		get { return id; }
		set { id = value; }
	}

	public string Title {
		get { return title; }
		set { title = value; }
	}

	public int StartCash {
		get { return startCash; }
		set {startCash  = value; }
	}

	public int TargetWorth {
		get { return targetWorth; }
		set { targetWorth = value; }
	}

	public int MinPlayers {
		get { return minPlayers; }
		set { minPlayers = value; }
	}

	public int MaxPlayers {
		get { return maxPlayers; }
		set { maxPlayers = value; }
	}

	public List<TileInfo> Tiles {
		get { return tiles; }
	}

	public List<string> Districts {
		get { return districts; }
		private set { districts = value; }
	}
	
	public BoardInfo() {}

	public BoardInfo(string id, string title, int startCash, int targetWorth, int minPlayers, int maxPlayers, List<TileInfo> tiles) {
		ID = id;
		Title = title;
		StartCash = startCash;
		TargetWorth = targetWorth;
		MinPlayers = minPlayers;
		MaxPlayers = maxPlayers;
		this.tiles = new List<TileInfo>();
		this.districts = new List<string>();
		foreach (TileInfo tile in tiles) {
			Tiles.Add(tile);
			if(tile.District != null && !districts.Contains(tile.District)) {
				districts.Add(tile.District);
			}
		}
	}

	public static BoardInfo FromJSON(JSONNode json) {
		List<TileInfo> tiles = new List<TileInfo>();
		for (int i = 0; i < json["tiles"].Count; i++) {
			tiles.Add(TileInfo.FromJSON(json["tiles"][i]));
		}
		return new BoardInfo(json["id"].Value, json["title"].Value, json["startCash"].AsInt, json["targetWorth"].AsInt, json["minPlayers"].AsInt, json["maxPlayers"].AsInt, tiles);
	}
}

public class TileInfo {
	private Constants.TileCodes code;
	private int tileX;
	private int tileY;
	private string district;
	private int shopValue;
	private string propertyName;
	private List<DirRestriction> restrictions;
	
	public Constants.TileCodes Code {
		get { return code; }
		set { code = value; }
	}
	
	public int TileX {
		get { return tileX; }
		set { tileX = value; }
	}
	
	public int TileY {
		get { return tileY; }
		set { tileY = value; }
	}
	
	public string District {
		get { return district; }
		set { district = value; }
	}
	
	public int ShopValue {
		get { return shopValue; }
		set { shopValue = value; }
	}
	
	public string PropertyName {
		get { return propertyName; }
		set { propertyName = value; }
	}
	
	public List<DirRestriction> Restrictions {
		get { return restrictions; }
	}
	
	public TileInfo() {}
	
	public TileInfo(Constants.TileCodes tileCode, int x, int y, List<DirRestriction> restrictions, string district = null, string name = null) {
		Code = tileCode;
		TileX = x;
		TileY = y;
		if(tileCode == Constants.TileCodes.Property) {
			District = district;
		}
		ShopValue = Random.Range(100, 500);
		PropertyName = name;
		this.restrictions = new List<DirRestriction> ();
		foreach (DirRestriction restriction in restrictions) {
			Restrictions.Add(restriction);
		}
	}

	public static TileInfo FromJSON(JSONNode json) {
		List<DirRestriction> restrictions = new List<DirRestriction>();
		if (false) {
			foreach (KeyValuePair<string, JSONNode> restriction in (JSONClass)json["dirRestrictions"].AsObject) {
				DirRestriction newRestriction = DirRestriction.FromJSON (restriction.Key, restriction.Value);
				restrictions.Add (newRestriction);
			}
		}
		return new TileInfo((Constants.TileCodes)json["tileCode"].AsInt, json["x"].AsInt, json["y"].AsInt, restrictions, json["district"].Value, json["name"].Value);
	}
}

public class DirRestriction {
	private Constants.Directions direction;
	private List<List<Constraint>> constraints;
	
	public Constants.Directions Direction {
		get { return direction; }
		set { direction = value; }
	}
	
	public List<List<Constraint>> Constraints {
		get { return constraints; }
	}

	public DirRestriction() {}

	public DirRestriction(int dir, List<List<Constraint>> constraints) {
		Direction = (Constants.Directions)dir;
		this.constraints = new List<List<Constraint>> ();
		foreach (List<Constraint> constraint in constraints) {
			Constraints.Add(constraint);
		}
	}
	
	public static DirRestriction FromJSON(string dir, JSONNode json) {
		List<List<Constraint>> constraints = new List<List<Constraint>>();
		for (int i = 0; i < json.Count; i++) {
			List<Constraint> newConstraints = new List<Constraint>();
			foreach (KeyValuePair<string, JSONNode> constraint in json[i].AsObject) {
				Constraint newConstraint = new Constraint(constraint.Key, constraint.Value);
				newConstraints.Add(newConstraint);
			}
			constraints.Add(newConstraints);
		}
		return new DirRestriction(System.Convert.ToInt32(dir), constraints);
	}
	
	public void AddConstraints(List<Constraint> constraints) {
		Constraints.Add(constraints);
	}
}

public class Constraint {
	private Constants.TileConstraints constraintType;
	private string compareValue;
	
	public Constants.TileConstraints ConstraintType {
		get { return constraintType; }
		set { constraintType = value; }
	}
	
	public string CompareValue {
		get { return compareValue; }
		set { compareValue = value; }
	}

	public Constraint() {}

	public Constraint(string type, string value) {
		ConstraintType = (Constants.TileConstraints)System.Convert.ToInt32(constraintType);
		CompareValue = value;
	}
}