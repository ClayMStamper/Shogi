using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Player 1 is trying to minimize
 * Player 2 is trying to maximize
 */

public class AI2 : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	public static AI2 instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	}

	public static AI2 GetInstance(){
		return instance;
	}

	#endregion

	public delegate void OnMovePickedCallback(Square move); 

	public bool isActive = true;

	[SerializeField]
	BoardManager boardManager;

	public List <GameObject> allPieces;

	Piece selectedPiece = null;
	Square selectedMove;

	int maxEval, minEval;                 

	public void Go(){

		Board currentBoard = new Board (boardManager.pieces);
		Minimax (currentBoard, 1, !boardManager.isPlayerOnesTurn);

		//selectedMove is set by PickPiece()
		DoMove (selectedMove);

	}

	void DoMove(Square moveTo){

		Debug.Log ("Piece selected to move: " + selectedPiece);
		selectedMove.Print ();

		boardManager.SelectPiece (selectedPiece);

		boardManager.MovePiece (selectedMove);

	}

	int Minimax(Board board, int depth, bool maximizing){

		if (depth == 0) {
			return board.eval;
		}

		List <Square> allMoves = new List<Square> ();

		//is player 1s turn
		if (!maximizing) {

			minEval = int.MinValue;

			allMoves = board.GetAllLegalMoves (true);

			foreach (Square moveTo in allMoves) {

				Square myRootSquare = moveTo;
				Piece myRootPiece = myRootSquare.moveFrom.Last ();

				int i = moveTo.moveFrom.Count;

				while (i > 0){
					
					i--;

					Square movedFrom = new Square (new Vector2Int (moveTo.moveFrom [i].x, moveTo.moveFrom [i].y));
					Board newBoard = new Board (board, moveTo, movedFrom);

					int eval = Minimax (newBoard, depth - 1, true);

					if (eval < minEval) { //found the better move
						minEval = eval;
						selectedMove = myRootSquare;
						selectedPiece = myRootPiece;
					}
					minEval = Mathf.Min (eval, minEval);
					return minEval;

				}
			}

			Debug.LogError ("No moves availabele: something is wrong");
			return 0;

		} else {

			maxEval = int.MaxValue;

			allMoves = board.GetAllLegalMoves (false);

			foreach (Square moveTo in allMoves) {

				int i = moveTo.moveFrom.Count;

				while (i > 0){

					i--;

					Square movedFrom = new Square (new Vector2Int (moveTo.moveFrom [i].x, moveTo.moveFrom [i].y));
					Board newBoard = new Board (board, moveTo, movedFrom);

					int eval = Minimax (newBoard, depth - 1, false);

					if (eval > maxEval) { //found the better move
						maxEval = eval;
//						nextMove = moveTo;
//						nextPiece = newBoard.pieces [moveTo.x, moveTo.y];
					}

					return maxEval;

				}
			}

			Debug.LogError ("No moves availabele: something is wrong");
			return 0;

		}
	}

}

// Abstraction of any coordinate on the board
public struct Square{

	public int x, y;
	public List <Piece> moveFrom;

//	public Board currentBoard;

	public Vector2Int pos{
		get{return new Vector2Int (x, y);}
		set{x = pos.x; y = pos.y;}
	}

	public Square (Vector2Int move){
		x = move.x;
		y = move.y;

		moveFrom = new List<Piece> ();
	}

	//converts vector array to Move array
	public static List<Square> coordsToMovesList(bool[,] moveIsLegalArr){

		List <Square> newMovesList = new List<Square> ();

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {

				if (moveIsLegalArr [i, j]) {
					newMovesList.Add (new Square (new Vector2Int (i, j)));
				}

			}
		}

		return newMovesList;

	}

	Board GetCurrentBoard(){

		Board board = new Board();



		return board;

	}

	public void Print(){
		Debug.Log ("available move: " + x + ", " + y);
	}

}

struct Board{

	public int eval;
	public Piece[,] pieces;

	//evaluate current board
	public Board (Piece [,] basePieces){

		//set default values
		pieces = basePieces;
		eval = 0;

		eval = Evaluate ();

	}

	//create new board
	public Board (Board baseBoard, Square moveTo, Square moveFrom){

		//set default values
		pieces = baseBoard.pieces;
		eval = 0;

		//adjust board and evaluate
		MovePiece (moveTo, moveFrom);
		eval = Evaluate ();

	}

	void MovePiece(Square moveTo, Square moveFrom){

		//copy piece to new square
		pieces [moveTo.x, moveTo.y] = pieces [moveFrom.x, moveFrom.y];

		//erase old square
		pieces [moveFrom.x, moveFrom.y] = null;

	}

	public List <Square> GetAllLegalMoves(bool isPlayerOne){

		List <Square> allMoves = new List<Square> ();

		if (isPlayerOne) { // is minimizing

			foreach (GameObject piece in AI2.GetInstance().allPieces) {

				if (piece.GetComponent<Piece> ().isPlayerOne) {

					List <Square> legalMoves = new List<Square> ();
					legalMoves = Square.coordsToMovesList (piece.GetComponent<Piece> ().LegalMoves ());

					foreach (Square move in legalMoves) {

						move.moveFrom.Add (piece.GetComponent<Piece> ());

						if (!allMoves.Contains (move)) {

							allMoves.Add (move);

						}
					}
				}
			}

		} else { // is maximizing

			foreach (GameObject piece in AI2.GetInstance().allPieces) {

				if (!piece.GetComponent<Piece> ().isPlayerOne) {

					List <Square> legalMoves = new List<Square> ();
					legalMoves = Square.coordsToMovesList (piece.GetComponent<Piece> ().LegalMoves ());

					foreach (Square move in legalMoves){

						move.moveFrom.Add (piece.GetComponent<Piece> ());

						if (!allMoves.Contains (move)) {

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
							score1 -= 90;
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
							score2 += 90;
						}
					}
				}

			}
		}

		Debug.Log ("Final score 1 is: " + score1);
		Debug.Log ("Final score 2 is: " + score2);

		return score1+score2;
	}

}
