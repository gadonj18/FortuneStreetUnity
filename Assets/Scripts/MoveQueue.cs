using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveList {
	private List<Move> moves;
	private List<Tile> queue;
	private int currentIdx;

	public MoveList() {
		moves = new List<Move>();
		currentIdx = 0;
	}
	
	public void AddMove(Move move) {
		moves.Add(move);
		currentIdx++;
	}

	public void AddPath(List<Move> moves) {
		moves.AddRange(moves);
		currentIdx++;
	}

	public void GoBack() {
		moves.RemoveRange(currentIdx, moves.Count - currentIdx);
		currentIdx--;
	}

	public void ClearQueue() {
		moves.RemoveRange(currentIdx + 1, moves.Count - currentIdx - 1);
	}
	
	public List<Move> GetQueue() {
		return moves.GetRange(currentIdx + 1, moves.Count - currentIdx - 1);
	}
	
	public List<Move> GetHistory() {
		return moves.GetRange(0, currentIdx + 1);
	}
	
	public List<Move> GetList() {
		return moves;
	}
}

public class Move {
	Tile tile;
	int cash = 0;
	KeyValuePair<string, int>? stock;
	Constants.Cards card;
	int level = 0;
	
	public Move(Tile newTile) {
		tile = newTile;
	}

	public Move(Tile newTile, int newCash, KeyValuePair<string, int> newStock, Constants.Cards newCard, int newLevel) {
		tile = newTile;
		cash = newCash;
		stock = newStock;
		card = newCard;
		level = newLevel;
	}
}