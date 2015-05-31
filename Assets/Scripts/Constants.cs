using System.Collections.Generic;
using UnityEngine;

public static class Constants {
	public enum Directions {
		N, NNW, NW, WNW, W, WSW, SW, SSW, S, SSE, SE, ESE, E, ENE, NE, NNE, Any };

	public enum TileCodes {
		Heart,
		Spade,
		Diamond,
		Club,
		Arcade,
		Dice,
		Warp,
		StockMarket,
		Bank,
		Sleep,
		Commission,
		Card,
		Property
	};

	public enum GameStates {
		GameInfo,
		PreviewBoard,
		DecideOrder,
		Playing,
		GameWon,
		Scoreboard
	}

	public enum TileConstraints { PrevDir };

	public enum Suits { Wild, Heart, Spade, Diamond, Club };

	public enum InputEvents {
		MouseClick,
		MouseHeld,
		MouseUp,
		KeyClick,
		KeyHeld,
		KeyUp
	}

	public static Dictionary<Suits, Color> SuitColors = new Dictionary<Suits, Color>() {
		{ Suits.Heart, new Color(234f / 256f, 114f / 256f, 199f / 256f, 256f) },
		{ Suits.Spade, new Color(0f, 38f / 256f, 235f / 256f, 256f) },
		{ Suits.Diamond, new Color(227f / 256f, 241f / 256f, 0f, 256f) },
		{ Suits.Club, new Color(0f, 189f / 256f, 53f / 256f, 256f) }
	};
}