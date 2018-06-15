using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField]
	List <GameObject> allPieces;

	Piece selectedPiece = null;
	Square selectedMove;

	int maxEval, minEval;                 

	public void Go(){
		
		PickPieceAndSquare ();

		//selectedMove is set by PickPiece()
		DoMove (selectedMove);

	}

	void PickPieceAndSquare(){

		Piece piece = null;
		int bestSquareEval = int.MaxValue; //defaulted to worst possible eval (lower is better for the AI)

		//replace this: it's random
		do {
			piece = allPieces[Random.Range (0, allPieces.Count)].GetComponent <Piece>();
		} while (!piece.isPlayerOne && piece != null);

		selectedPiece = piece;


	}

	Square CalculateSquare(){

		Square bestMove = new Square ();

		return bestMove;

	}

	void DoMove(Square moveTo){

		List <Square> legalMoves = new List<Square> ();
		legalMoves = Square.coordsToMovesList (selectedPiece.LegalMoves ());

		selectedMove = legalMoves [0];
		selectedMove.Print ();

		boardManager.SelectPiece (selectedPiece);

		boardManager.MovePiece (selectedMove);

	}

	int Minimax(Board board, int depth, bool maximizing){

		if (depth == 0) {
			board.eval;
		}

		//is player 1s turn
		if (maximizing) {
			return 0;
		}
		return 0;
	}

}

//"Move" nown: an available spot ont the board to move to
public struct Square{

	public int eval;
	public int x, y;

//	public Board currentBoard;

	public Vector2Int pos{
		get{return new Vector2Int (x, y);}
		set{x = pos.x; y = pos.y;}
	}

	Square (Vector2Int move){
		x = move.x;
		y = move.y;
		eval = 0;
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
	Board (Piece [,] basePieces){

		//set default values
		pieces = basePieces;
		eval = 0;

		eval = Evaluate ();

	}

	//create new board
	Board (Board baseBoard, Square moveTo, Square moveFrom){

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

		return score1+score2;
	}

}
