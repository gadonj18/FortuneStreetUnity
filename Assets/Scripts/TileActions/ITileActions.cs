using UnityEngine;
using System.Collections;

public interface ITileActions {
	IEnumerator PassTile(bool reversing = false);
	IEnumerator LandOnTile();
	IEnumerator LeaveTile();
	void MoveChanges(Move move);
	void FinishTurn();
}