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
	public Tile tile;
	public int cash = 0;
	public KeyValuePair<string, int>? stock = null;
	public Dictionary<Constants.Cards, bool> cards = new Dictionary<Constants.Cards, bool>();
	public int level = 0;
	
	public Move(Tile newTile) {
		tile = newTile;
	}
	
	public void CardGained(Constants.Cards card) {
		cards[card] = true;
	}
	
	public void CardLost(Constants.Cards card) {
		cards[card] = true;
	}
}