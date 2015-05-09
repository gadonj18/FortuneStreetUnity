using UnityEngine;
using System.Collections;

public abstract class BaseTileActions : MonoBehaviour, ITileActions {
	public Playing GameState;
	public GameObject Tile;
	public abstract IEnumerator PassTile(bool reversing = false);
	public abstract IEnumerator LandOnTile();
	public abstract IEnumerator LeaveTile();
	public virtual void MoveChanges(Move move) { return; }
	private void Awake() {
		Tile = transform.gameObject;
	}
}