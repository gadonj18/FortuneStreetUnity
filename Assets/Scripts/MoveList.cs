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
	
	public void Next(Move newMove) {
		moves.Add(newMove);
		queue.RemoveAt(0);
	}

	public void SaveMove(Move move) {
		moves.Add(move);
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
	public Tile fromTile;
	public Tile toTile;
	public int cash = 0;
	public KeyValuePair<string, int>? stock = null;
	public Dictionary<Constants.Suits, bool> cards = new Dictionary<Constants.Suits, bool>();
	public int level = 0;
	
	public void CardGained(Constants.Suits card) {
		cards[card] = true;
	}
	
	public void CardLost(Constants.Suits card) {
		cards[card] = true;
	}
}