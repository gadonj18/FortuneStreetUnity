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

	public enum Cards { Heart, Spade, Diamond, Club };

	public enum InputEvents {
		MouseClick,
		MouseHeld,
		MouseUp,
		KeyClick,
		KeyHeld,
		KeyUp
	}
}