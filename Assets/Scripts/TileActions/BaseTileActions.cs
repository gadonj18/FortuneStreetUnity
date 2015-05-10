using UnityEngine;
using System.Collections;

public abstract class BaseTileActions : MonoBehaviour, ITileActions {
	public Playing GameState;
	public GameObject Tile;
	public virtual IEnumerator MoveToTile(bool reversing = false) { yield break; }
	public virtual IEnumerator OnTile() { yield break; }
	public virtual IEnumerator LeaveTile() { yield break; }
	public virtual IEnumerator LandOnTile() { yield break; }
	public virtual void MoveChanges(Move move) { return; }
	public virtual void FinishTurn() { GameState.FinishTurn(); }
	private void Awake() { Tile = transform.gameObject; }
}