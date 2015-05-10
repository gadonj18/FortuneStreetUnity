using UnityEngine;
using System.Collections;

public interface ITileActions {
	IEnumerator MoveToTile(bool reversing = false);
	IEnumerator OnTile();
	IEnumerator LeaveTile();
	IEnumerator LandOnTile();
	void MoveChanges(Move move);
	void FinishTurn();
}