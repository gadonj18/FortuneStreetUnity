using UnityEngine;
using System.Collections.Generic;

public class Config {
	private static Config instance;
	private Config() {}
	public static Config Instance {
		get {
			if (instance == null) {
				instance = new Config();
			}
			return instance;
		}
	}

	private Dictionary<string, Color> districtColors = new Dictionary<string, Color>() {
		{ "A", Color.red },
		{ "B", Color.cyan },
		{ "C", Color.green },
		{ "D", Color.black },
		{ "E", Color.blue },
		{ "F", Color.magenta },
		{ "G", Color.gray },
		{ "H", Color.white },
		{ "I", Color.yellow }
	};

	public Color GetDistrictColor(string DistrictCode) {
		return districtColors[DistrictCode];
	}

	public void SetDistrictColor(string DistrictCode, Color color) {
		if (!districtColors.ContainsKey(DistrictCode)) {
			districtColors.Add(DistrictCode, color);
		} else {
			districtColors [DistrictCode] = color;
		}
	}

	public Dictionary<int, PlayerInfo> playerInfo = new Dictionary<int, PlayerInfo>() {
		{ 1, new PlayerInfo("Player1", Color.red) },
		{ 2, new PlayerInfo("Player2", Color.blue) },
		{ 3, new PlayerInfo("Player3", Color.green) },
		{ 4, new PlayerInfo("Player4", Color.white) }
	};
}

public struct PlayerInfo {
	public string Name;
	public Color Color;

	public PlayerInfo(string name, Color color) {
		Name = name;
		Color = color;
	}
}