using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveList {
	private List<Move> moves;
	private List<Tile> queue;

	public MoveList() {
		moves = new List<Move>();
		queue = new List<Tile>();
	}
	
	public List<Tile> Queue {
		get { return queue; }
		set { AddQueue(value); }
	}

	public List<Move> Moves {
		get { return moves; }
		private set { moves = value; }
	}
	
	public void Next() {
		Move newMove = new Move(queue[0]);
		queue.RemoveAt(0);
	}

	public void FinishMove(int cashChange, KeyValuePair<string, int> stockChange, Constants.Cards? cardChange, int levelChange) {
		moves[moves.Count - 1].SetChanges(cashChange, stockChange, cardChange, levelChange);
	}

	public void AddQueue(List<Tile> tiles) {
		queue.Clear();
		foreach(Tile tile in tiles) {
			queue.Add(tile);
		}
	}

	public void GoBack() {
		queue.Clear();
		moves.RemoveAt(moves.Count - 1);
	}

	public void ClearQueue() {
		queue.Clear();
	}

	public Move LastMove() {
		return moves[moves.Count - 1];
	}
}

public class Move {
	public Tile tile;
	public int cash = 0;
	public KeyValuePair<string, int>? stock = null;
	public Constants.Cards? card = null;
	public int level = 0;
	
	public Move(Tile newTile) {
		tile = newTile;
	}

	public void SetChanges(int cashChange, KeyValuePair<string, int>? stockChange, Constants.Cards? cardChange, int levelChange) {
		cash = cashChange;
		stock = stockChange;
		card = cardChange;
		level = levelChange;
	}
}