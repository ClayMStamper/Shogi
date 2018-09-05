using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board{

	public int eval;
	public Piece[,] pieces;
	public List <Square> playerOneMoves;
	public List <Square> playerTwoMoves;
	public SideTable sideTable;

	public Board (){

		pieces = new Piece[,] { 
			{new Lancer(), new Knight(), new Silver(), new Gold(), new King(), new Gold(), new Silver(), new Knight(), new Lancer()},
			{null, new Bishop(), null, null, null, null, null, new Rook(), null},
			{new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn()}, 
			{null, null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null, null},
			{new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn(), new Pawn()},
			{null, new Bishop(), null, null, null, null, null, new Rook(), null},
			{new Lancer(), new Knight(), new Silver(), new Gold(), new King(), new Gold(), new Silver(), new Knight(), new Lancer()} };

		playerOneMoves = GetAllLegalMoves (true); // {new Square()};
		playerTwoMoves = GetAllLegalMoves (false);

		sideTable = new SideTable (BoardManager.GetInstance().table1);
	}

	//evaluate current board
	public Board (Piece [,] basePieces){

		//set default values
		pieces = (Piece[,])basePieces.Clone();
		sideTable = new SideTable (BoardManager.GetInstance().table1);
		playerOneMoves = new List<Square> ();
		playerTwoMoves = new List<Square> ();
		eval = 0;

		playerOneMoves = GetAllLegalMoves (isPlayerOnesTurn: true);
		playerTwoMoves = GetAllLegalMoves (isPlayerOnesTurn: false);
		eval = Evaluate ();

	}

	//create new board
	public Board (Board baseBoard, Square moveTo, Square moveFrom, Piece moved){

		//	Piece piece = AI.GetInstance ().selectedPiece;

		//set default values
		pieces = (Piece[,])baseBoard.pieces.Clone();
		sideTable = baseBoard.sideTable;
		playerOneMoves = new List<Square> ();
		playerTwoMoves = new List<Square> ();
		eval = 0;

		//	Debug.Log ("Created new board with piece: " + moved.name + "and moveTo: " + moveTo.x + ", " + moveTo.y);

		//adjust board and evaluate

		if (moved.isCaptured) {
			DropPiece (moveTo, moved);
		} else {
			//		Debug.Log ("Before move, " + moveTo.x + ", " + moveTo.y + " is: " + this.pieces[moveTo.x, moveTo.y]);
			MovePiece (moveTo, moveFrom, moved);
			//		Debug.Log ("After move, " + moveTo.x + ", " + moveTo.y + " is: " + this.pieces[moveTo.x, moveTo.y]);
		}

		playerOneMoves = GetAllLegalMoves (isPlayerOnesTurn: true);
		playerTwoMoves = GetAllLegalMoves (isPlayerOnesTurn: false);
		this.eval = Evaluate ();

	}

	void MovePiece(Square moveTo, Square moveFrom, Piece moved){

		//copy piece to new square
		this.pieces [moveTo.x, moveTo.y] = moved;

		//erase old square
		this.pieces [moveFrom.x, moveFrom.y] = null;

	}

	void DropPiece (Square moveTo, Piece moved){

		//copy piece to its new square
		this.pieces[moveTo.x, moveTo.y] = moved;

		//Remove from sideTable
		//	if (sideTable.sidePieces.Contains (moved)) {
		//		Debug.Log ("Piece that was dropped was found on side table! Dank.");
		this.sideTable.sidePieces.Remove (moved);
		//	} else {
		//		Debug.LogError ("Tried to drop piece that wasn't found on the side table. Check table constructor");
		//	}

	}

	public List <Square> GetAllLegalMoves(bool isPlayerOnesTurn){

		List <Square> allMoves = new List<Square> ();

		//whos turn is it
		if (isPlayerOnesTurn) {
			// (将棋);
			//check every piece(将棋)
			for (int i = 0; i < 9; i++){
				for (int j = 0; j < 9; j++) { 

					Piece p = pieces [i, j];

					//piece exists and piece is mine
					if (p != null && p.isPlayerOne) { 

						List <Square> thisPiecesMoves = new List<Square> ();

						if (!p.isCaptured) {
							thisPiecesMoves = p.LegalMovesList(this);
						} else {
							thisPiecesMoves = p.LegalDropsList(this);
						}

						foreach (Square move in thisPiecesMoves) {
							allMoves.Add (move);
						}

					}
				} 
			}
		} else { // is maximizing
			for (int i = 0; i < 9; i++){
				for (int j = 0; j < 9; j++) { //for each piece

					Piece p = pieces [i, j];

					//piece exists and piece is mine
					if (p != null && !p.isPlayerOne) {

						List <Square> thisPiecesMoves = new List<Square> ();

						if (!pieces [i, j].isCaptured) {
							thisPiecesMoves = p.LegalMovesList(this);
						} else {
							thisPiecesMoves = p.LegalDropsList(this);
						}

						foreach (Square move in thisPiecesMoves) {
							allMoves.Add (move);
						}

					}
				}

			}
		}

		return allMoves;

	}

	int Evaluate(){

		int score1 = 0, score2 = 0;

		for(int i = 0; i < 9; i++){
			for(int j=0; j < 9; j++){

				if (pieces [i, j] != null) {

					Piece piece = pieces [i, j];

					if (pieces [i, j].isPlayerOne) { //case that piece AI
						if (piece is Dragon) {
							score1 -= 12;
						} else if (piece is Gryphon || piece is Rook) {
							score1 -= 10;
						} else if (piece is Bishop) {
							score1 -= 8;
						} else if (piece is GoldPromo && piece.gameObject.name == "Pawn") {
							score1 -= 7;
						} else if (piece is Gold || piece is GoldPromo) {
							score1 -= 6;
						} else if (piece is Silver) {
							score1 -= 5;
						} else if (piece is Knight) {
							score1 -= 4;
						} else if (piece is Lancer) {
							score1 -= 3;
						} else if (piece is Pawn) {
							score1 -= 1;
						} else if (piece is King) {
							score1 -= 900;
						}

					} else { //case that piece is player
						if (piece is Dragon) {
							score2 += 12;
						} else if (piece is Gryphon || piece is Rook) {
							score2 += 10;
						} else if (piece is Bishop) {
							score2 += 8;
						} else if (piece is GoldPromo && piece.gameObject.name == "Pawn") {
							score2 += 7;
						} else if (piece is Gold || piece is GoldPromo) {
							score2 += 6;
						} else if (piece is Silver) {
							score2 += 5;
						} else if (piece is Knight) {
							score2 += 4;
						} else if (piece is Lancer) {
							score2 += 3;
						} else if (piece is Pawn) {
							score2 += 1;
						} else if (piece is King) {
							score2 += 900;
						}
					}
				}

			}
		}

		//	Debug.Log ("Final score 1 is: " + score1);
		//	Debug.Log ("Final score 2 is: " + score2);

		return score1+score2;
	}

	public void Print(){

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {
				Debug.Log ("Piece at " + i + ", " + j + ": " + pieces [i, j]);
			}
		}

	}

}

public class Square{

	public int x, y;
	public Piece pieceMoving;

	//	public Board currentBoard;

	public Square (){
		x = 0;
		y = 0;
		pieceMoving = null;
	}

	public Vector2Int pos{
		get{return new Vector2Int (x, y);}
		set{x = pos.x; y = pos.y;}
	}

	public Square (Vector2Int move){

		x = move.x;
		y = move.y;

		pieceMoving = null;

		//	Debug.Log ("Created new square at " + x + ", " + y);

	}

	public Square (Vector2Int move, Piece piece){

		x = move.x;
		y = move.y;

		this.pieceMoving = piece;

		//	Debug.Log ("Created new square at " + x + ", " + y);

	}

	public void Print(){
		Debug.Log ("available move: " + x + ", " + y + ")");
	}

}

public struct SideTable {

	public List <Piece> sidePieces;

	public SideTable (SideTableManager table){

		sidePieces = new List<Piece> ();

		for (int i = 3; i < 0; i++) {
			for (int j = 3; j < 0; j++) {

				foreach (Piece piece in table.pieceStacks[i,j]) {
					sidePieces.Add (piece);
				}

			}
		}
	}

}
