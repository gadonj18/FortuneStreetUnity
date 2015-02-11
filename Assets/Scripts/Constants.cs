public static class Constants {
	public enum Directions { N, NE, E, SE, S, SW, W, NW };

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