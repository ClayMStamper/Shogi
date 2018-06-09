using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	public static AI instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	}

	public static AI GetInstance(){
		return instance;
	}

	#endregion

	public Piece[] allPieces;

	//bool [,] legalMoves = new bool[9,9];
	Vector2Int[] moves; 

	public bool isActive = true;

	BoardManager board;
	Piece selectedPiece = null;
	Vector2Int selectedMove;
	int maxEval, minEval;

	public int difficulty = 1;

	void Start (){

		board = BoardManager.GetInstance ();

	}

	public void Go(){

		CalculateMove ();

		//set the BoardManager coord to movePos coord
		board.selectedX = selectedMove.x;
		board.selectedY = selectedMove.y;

		board.MovePiece ();


	}

	void CalculateMove(){

		int bestEval = int.MinValue;

		Piece bestPiece = null;

		//for a given piece
		foreach (Piece piece in allPieces) {

			//gets the moves that will serve as roots in the 
			//recursive minimax binary tree
			moves = ListMoves (piece.LegalMoves ());

			//indexer that's used to back track
			// from best value to best move
			int i = 0;

			//if piece belongs to me
			if (piece.isPlayerOne) {
				
				Vector2Int[] moves = ListMoves(piece.LegalMoves ());
				List <Piece[,]> possibleBoards = new List<Piece[,]> ();
				List <int> boardEvals = new List<int> ();

				selectedPiece = piece;

				//translate the pieces possible moves into
				//respective hypothetical boards
				foreach (Vector2Int move in moves) {
					possibleBoards.Add (PossibleBoard (move));
				}

				//hypothetical boards are given assigned a value through the
				//recursive minimax algorithm
				foreach (Piece[,] board in possibleBoards) {
					boardEvals.Add (Minimax (board, 1, true));
				}

				//the highest ranked board is back tracked to piece and move
				foreach (int val in boardEvals) {
					
					i++;
					if (board.isPlayerOnesTurn) { //is maximizing
						if (val >= bestEval) {

							bestPiece = piece;
							selectedMove = moves [i];

						}
					} else { //is minimizing

						if (val <= bestEval) {

							bestPiece = piece;
							selectedMove = moves [i];

						}

					}
				}
			}
		}

		board.SelectPiece (bestPiece.x, bestPiece.y);

	}

	//player 1 tries to maximize, 2 tries to minimize
	int Minimax(Piece[,] board, int depth, bool maximizing){

		if (depth == 0) {
			//reached the root: return the root boards value
			return Evaluate (board);
		}

		//is player 1s turn
		if (maximizing) {

			maxEval = int.MinValue;

			foreach (Vector2Int move in moves) {

				//possible moves for next recursion will be the list of legal moves of the piece
				//at the location that was just hypothetically moved to
		//		Piece[,] possibleBoard = PossibleBoard (move);
		//		moves = ListMoves (PossibleBoard (move)[move.x, move.y].LegalMoves());

				int eval = Minimax (PossibleBoard(move), depth - 1, false);
				maxEval = Mathf.Max (maxEval, eval);

			}

			return maxEval;

		} else {

			minEval = int.MaxValue;

			foreach (Vector2Int move in moves) {

		//		moves = ListMoves (PossibleBoard (move)[move.x, move.y].LegalMoves());

				int eval = Minimax (PossibleBoard(move), depth - 1, true);
				minEval = Mathf.Min (minEval, eval);

			}

			return minEval;

		}
	}

	//takes a possible move and turns it into a possible board
	Piece[,] PossibleBoard(Vector2Int move){

		Piece[,] board = this.board.pieces;
		Piece piece = board [selectedPiece.x, selectedPiece.y];
		board [selectedPiece.x, selectedPiece.y] = null;
		board [move.x, move.y] = piece;

		return board;

	}

	public int Evaluate (Piece[,] board){

		int score1 = 0, score2 = 0;

		for(int i = 0; i < 9; i++){
			for(int j=0; j < 9; j++){

				if (board [i, j] != null) {
					
					Piece piece = board [i, j];

					if (board [i, j].isPlayerOne) { //case that piece AI
						if (piece is Dragon) {
							score1 += 12;
						} else if (piece is Gryphon || piece is Rook) {
							score1 += 10;
						} else if (piece is Bishop) {
							score1 += 8;
						} else if (piece is GoldPromo && piece.gameObject.name == "Pawn") {
							score1 += 7;
						} else if (piece is Gold || piece is GoldPromo) {
							score1 += 6;
						} else if (piece is Silver) {
							score1 += 5;
						} else if (piece is Knight) {
							score1 += 4;
						} else if (piece is Lancer) {
							score1 += 3;
						} else if (piece is Pawn) {
							score1 += 1;
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
						} 
					}
				}

			}
		}

		Debug.Log ("Final score 1 is: " + score1);
		Debug.Log ("Final score 2 is: " + score2);

		return score1-score2;

	}

	Vector2Int[] ListMoves(bool[,] legalMoves){

		List <Vector2Int> movesList = new List<Vector2Int> ();

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {

				movesList.Add (new Vector2Int (i, j));

			}
		}

		return movesList.ToArray ();

	}

	public void SetDifficulty(float difficulty){
		this.difficulty = (int)difficulty;
	}

	public void SetIsActive(bool isActive){
		this.isActive = isActive;
	}

}
