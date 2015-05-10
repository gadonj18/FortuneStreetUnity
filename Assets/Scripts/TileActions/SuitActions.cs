using UnityEngine;
using System.Collections;

public class SuitActions : BaseTileActions {
	private BaseSuit TileScript;

	private void Start() {
		TileScript = Tile.GetComponent<BaseSuit>();
	}

	public override IEnumerator MoveToTile(bool reversing = false) {
		GameState.PlayerScript.Suits[TileScript.Suit] = true;
		yield return null;
	}

	public override void MoveChanges(Move move) {
		move.cards[TileScript.Suit] = true;
	}

	public override IEnumerator LandOnTile() {
		GameState.FinishTurn();
		yield break;
	}
}