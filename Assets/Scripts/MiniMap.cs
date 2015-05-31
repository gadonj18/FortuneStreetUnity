using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {
	public Playing GameState;
	private Sprite[] miniMapIcons;
	private string[] spriteNames;
	private Dictionary<int, Dictionary<int, GameObject>> icons = new Dictionary<int, Dictionary<int, GameObject>>();
	private float iconSize = 50f;
	private float maxHeight = 250f;
	private GameObject location;
	private Vector3 initPos;
	private float scale = 1f;
	private Dictionary<int, Dictionary<int, float>> arrowRotations = new Dictionary<int, Dictionary<int, float>>() {
		{ -2, new Dictionary<int, float>() {
				{ -2, 90f }, //SW
				{ -1, 67.5f }, //WSW
				{ 0, 45f }, //W
				{ 1, 22.5f }, //WNW
				{ 2, 0f }, //NW
			}
		},
		{ -1, new Dictionary<int, float>() {
				{ -2, 112.5f }, //SSW
				{ 2, -22.5f }, //NNW
			}
		},
		{ 0, new Dictionary<int, float>() {
				{ -2, 135f }, //S
				{ 2, -45f }, //N
			}
		},
		{ 1, new Dictionary<int, float>() {
				{ -2, 157.5f }, //SSE
				{ 2, -67.5f }, //NNE
			}
		},
		{ 2, new Dictionary<int, float>() {
				{ -2, -180f }, //SE
				{ -1, -157.5f }, //ESE
				{ 0, -135f }, //E
				{ 1, -112.5f }, //ENE
				{ 2, -90f }, //NE
			}
		}
	};

	public void Awake () {
		PlayerController.PlayerMove += new PlayerController.PlayerMoveHandler(UpdatePlayerLocation);

		miniMapIcons = Resources.LoadAll<Sprite>(@"Sprites/MiniMap");

		//Store a list of icon names so we don't need to look them up by index
		spriteNames = new string[miniMapIcons.Length];
		for(int i = 0; i < miniMapIcons.Length; i++) {
			spriteNames[i] = miniMapIcons[i].name;
		}
		
		//location = miniMapIcons[System.Array.IndexOf<string>(spriteNames, "Location")];
		//arrow = miniMapIcons[System.Array.IndexOf<string>(spriteNames, "Arrow")];

		StartCoroutine("FlashLocation");
	}

	public void SwitchTurns() {
		location.GetComponent<Image>().color = GameState.PlayerScript.Color;
		UpdatePlayerLocation(null);
	}

	private void UpdatePlayerLocation(PlayerMoveEventArgs e) {
		location.GetComponent<RectTransform>().localPosition = initPos + new Vector3(GameState.PlayerScript.CurrentTile.BoardX * iconSize / 2f, GameState.PlayerScript.CurrentTile.BoardY * iconSize / 2f, 0f);
		if(GameState.PlayerScript.Direction == Constants.Directions.Any) {
			location.GetComponent<Image>().sprite = miniMapIcons[System.Array.IndexOf<string>(spriteNames, "Location")];
		} else {
			location.GetComponent<Image>().sprite = miniMapIcons[System.Array.IndexOf<string>(spriteNames, "Arrow")];
			Vector2 dir = GameState.Board.DirToXY[GameState.PlayerScript.Direction];
			location.transform.localRotation = Quaternion.Euler(0f, 0f, arrowRotations[(int)dir.x][(int)dir.y]);
		}
	}

	private IEnumerator FlashLocation() {
		while(location == null) yield return null;

		while(true) {
			for (float f = 1f; f >= 0; f -= 0.02f) {
				Color c = location.gameObject.GetComponent<Image>().color;
				c.a = f;
				location.gameObject.GetComponent<Image>().color = c;
				yield return null;
			}
			for (float f = 0f; f <= 1f; f += 0.02f) {
				Color c = location.gameObject.GetComponent<Image>().color;
				c.a = f;
				location.gameObject.GetComponent<Image>().color = c;
				yield return null;
			}
		}
	}

	/// <summary>
	/// Using the Board object, construts the mini map UI element
	/// </summary>
	public void BuildMiniMap(Board board) {
		//Size the minimap based on the board dimensions
		float width = (board.MaxX + 3f) * iconSize / 2f;
		float height = (board.MaxY + 3f) * iconSize / 2f;
		RectTransform rect = UIManager.Instance.MiniMap.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(width, height);

		initPos = new Vector3(0f - width / 2f + iconSize / 2f, 0f - height / 2f + iconSize / 2f, 0f);
		foreach(KeyValuePair<int, Dictionary<int, GameObject>> row in board.Tiles) {
			foreach(KeyValuePair<int, GameObject> col in row.Value) {
				Tile tile = col.Value.GetComponent<Tile>();
				string iconName = col.Value.GetComponent<Tile>().Type.ToString();
				Sprite icon = miniMapIcons[System.Array.IndexOf<string>(spriteNames, iconName)];
				string objName = iconName;
				if(tile.Type == Constants.TileCodes.Property) objName = objName + "_" + ((Property)tile).District;
				GameObject image = new GameObject(iconName);
				image.transform.SetParent(UIManager.Instance.MiniMap.transform);
				image.AddComponent<Image>();
				image.GetComponent<Image>().sprite = icon;
				RectTransform iconRect = image.GetComponent<RectTransform>();
				iconRect.sizeDelta = new Vector2(iconSize, iconSize);
				iconRect.anchorMin = new Vector2(0f, 0f);
				iconRect.anchorMax = new Vector2(0f, 0f);
				iconRect.localPosition = initPos + new Vector3(tile.BoardX * iconSize / 2f, tile.BoardY * iconSize / 2f, 0f);
				if(!icons.ContainsKey(tile.BoardY)) icons.Add(tile.BoardY, new Dictionary<int, GameObject>());
				icons[tile.BoardY].Add(tile.BoardX, image);
				if(tile.Type == Constants.TileCodes.Property) {
					Color oldColor = Config.Instance.GetDistrictColor(((Property)tile).District);
					Color newColor = new Color(oldColor.r + 0.3f, oldColor.g + 0.3f, oldColor.b + 0.3f);
					image.GetComponent<Image>().color = newColor;
				}
			}
		}
		
		//Create location icon
		location = new GameObject("Location");
		location.transform.SetParent(UIManager.Instance.MiniMap.transform);
		location.AddComponent<Image>();
		location.GetComponent<Image>().sprite = miniMapIcons[System.Array.IndexOf<string>(spriteNames, "Location")];
		RectTransform locationRect = location.GetComponent<RectTransform>();
		locationRect.sizeDelta = new Vector2(iconSize, iconSize);
		locationRect.anchorMin = new Vector2(0f, 0f);
		locationRect.anchorMax = new Vector2(0f, 0f);
		locationRect.localPosition = initPos + new Vector3(board.bank.GetComponent<Tile>().BoardX * iconSize / 2f, board.bank.GetComponent<Tile>().BoardY * iconSize / 2f, 0f);
		
		//Scale down the mini map if it's too big
		if(height > maxHeight) {
			scale = maxHeight / height;
			rect.localScale = new Vector3(scale, scale, 1f);
		}
		rect.position = new Vector3(width * scale / 2f + 10f, height * scale / 2f + 10f, 0f);
	}
}
