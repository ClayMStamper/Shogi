using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Player 1 is trying to minimize
 * Player 2 is trying to maximize
 */

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

	public int recursionDepth = 2;

	public delegate void OnMovePickedCallback(Square move); 

	public bool isActive = true;

	[SerializeField]
	BoardManager boardManager;

	public List <GameObject> allPieces;

	[HideInInspector]
	public Piece selectedPiece = null;
	Square selectedMove;

	int maxEval, minEval;                 

	public void Go(){

		Board currentBoard = new Board (boardManager.pieces);
		Minimax (currentBoard, recursionDepth, !boardManager.isPlayerOnesTurn, int.MinValue, int.MaxValue);

		//selectedMove is set in minimax
		DoMove ();

	}

	void DoMove(){

		Debug.Log ("Piece selected to move: " + selectedPiece);
		selectedMove.Print ();

		if (selectedPiece == null) {
			Debug.LogError ("FATAL: TRYING TO MOVE A NULL PIECE");
		}

		boardManager.SelectPiece (selectedPiece);
		boardManager.selectedX = selectedMove.x;
		boardManager.selectedY = selectedMove.y;
		boardManager.legalMoves = selectedPiece.LegalMoves ();

		if (!boardManager.legalMoves [selectedMove.x, selectedMove.y]) {
			Debug.LogError ("Fatal: AI is about to make an illegal move");
		}

		if (!selectedPiece.isCaptured) { //moving piece
			boardManager.legalMoves = selectedPiece.LegalMoves ();
			boardManager.MovePiece (selectedMove);
		} else { //dropping piece
			boardManager.legalMoves = GetLegalDrops (selectedPiece);
			boardManager.table1.DropPiece (selectedMove.x, selectedMove.y);
		}

	}

	int Minimax(Board board, int depth, bool maximizing, int alpha, int beta){

		Debug.Log ("Running minimax");

		if (depth == 0) {
			return board.eval;
		}

		List <Square> allSquaresToMoveTo = new List<Square> ();

		//is player 1s turn
		if (!maximizing) {

			minEval = int.MaxValue;

			allSquaresToMoveTo = board.GetAllLegalMoves (true);

			foreach (Square squareToMoveTo in allSquaresToMoveTo) {

			//	if (Random.value <= skipRate) {
			//		Debug.Log ("Skipping");
				//	break;
			//	}

				Square root = squareToMoveTo;
				GameObject rootPiece = root.piecesMoving.Last ();

				//# of pieces that can move to the same square
				//moveTo refers to the square, pieceMoved refers to the piece
				int j = squareToMoveTo.piecesMoving.Count;

				//will probably just iterate once unless multiple pieces are looking at the same spot
				while (j > 0){
					
					j--;

					//the square that the piece hypothetically moves from
					Square moveFrom = new Square (new Vector2Int (squareToMoveTo.piecesMoving [j].GetComponent<Piece>().x, squareToMoveTo.piecesMoving [j].GetComponent<Piece>().y));
					Board newBoard = new Board (board, squareToMoveTo, moveFrom, squareToMoveTo.piecesMoving[j].GetComponent<Piece>());

					int eval = Minimax (newBoard, depth - 1, true, alpha, beta);

					Debug.Log ("Eval is: " + eval);
					Debug.Log ("Min eval is: " + minEval);

					if (eval < beta) { //found the better move
						Debug.Log ("New piece should be selected");
						minEval = eval;
						beta = minEval;
						if (beta <= alpha) {
							break;
						}
						selectedMove = root;
						selectedPiece = rootPiece.GetComponent<Piece>();

					} else if (eval == minEval) {
						// this just adds in variability
						if (Random.value > 0.5f) {
							Debug.Log ("New piece should be selected");
							minEval = eval;
							selectedMove = root;
							selectedPiece = rootPiece.GetComponent<Piece>();
						}
					}
				}
			}

			return minEval;

		//	Debug.LogError ("No moves availabele: something is wrong");
		//	return 0;

		} else {

			maxEval = int.MinValue;

			allSquaresToMoveTo = board.GetAllLegalMoves (false);

			foreach (Square moveTo in allSquaresToMoveTo) {

			//	if (Random.value <= skipRate) {
			//		Debug.Log ("Skipping");
			//		break;
			//	}

				int i = moveTo.piecesMoving.Count;

				while (i > 0){

					i--;

					Square movedFrom = new Square (new Vector2Int (moveTo.piecesMoving [i].GetComponent<Piece>().x, moveTo.piecesMoving [i].GetComponent<Piece>().y));
					Board newBoard = new Board (board, moveTo, movedFrom, moveTo.piecesMoving[i].GetComponent<Piece>());

					int eval = Minimax (newBoard, depth - 1, false, alpha, beta);

					if (eval > alpha) { //found the better move
						maxEval = eval;
						alpha = maxEval;
						if (beta <= alpha) {
							break;
						}
//						
					//	if (eval >= goodEnoughEval) {
						//	return eval;
					//	}

					}
				}
			}

			return maxEval;

		}
	}

	public bool[,] GetLegalDrops(Piece piece){

		bool[,] legalMoves = new bool[9, 9];

		if (piece.name != "Pawn") { // not a pawn

			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 9; j++) {

					if (!PosIsBlocked (i, j)) {

						legalMoves [i, j] = true;

						//Debug.Log (i + ", " + j + ": Is Moveable");

					} else {

						legalMoves [i, j] = false;

					}
				}
			}

		} else { //is a pawn: apply special rules

			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 9; j++) {

					if (PosIsBlocked (i, j)) {

						legalMoves [i, j] = false;

						if (boardManager.pieces [i, j] is Pawn && 
							boardManager.pieces[i, j].isPlayerOne == piece.isPlayerOne) {

							for (int k = 0; k < 9; k++) {

								//sets moves in the file to illegal if a friendly
								//pawn is in that file
								legalMoves [i, k] = false;

							}

							break;

						}

					} else {

						legalMoves [i, j] = true;

					}
				}
			}
		}

		return legalMoves;

	}

	bool PosIsBlocked (int x, int y){

		if (boardManager.pieces [x, y] != null) {
			return true;
		} else {
			return false;
		}

	}

	public void ToggleIsActive(bool isActive){
		this.isActive = isActive;
	}

}

// Abstraction of any coordinate on the board
public struct Square{

	public int x, y;
	public List <GameObject> piecesMoving;

//	public Board currentBoard;

	public Vector2Int pos{
		get{return new Vector2Int (x, y);}
		set{x = pos.x; y = pos.y;}
	}

	public Square (Vector2Int move){

		x = move.x;
		y = move.y;

		piecesMoving = new List<GameObject> ();

	//	Debug.Log ("Created new square at " + x + ", " + y);

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

	public void Print(){
		Debug.Log ("available move: " + x + ", " + y + ")");
	}

}

public struct Board{

	public int eval;
	public Piece[,] pieces;
	public List <Piece> pieceList;
	SideTable sideTable;

	//evaluate current board
	public Board (Piece [,] basePieces){

		//set default values
		pieces = basePieces;
		pieceList = new List<Piece> ();
		sideTable = new SideTable (BoardManager.GetInstance().table1);
		eval = 0;

		GetPieceList ();
		eval = Evaluate ();

	}

	//create new board
	public Board (Board baseBoard, Square moveTo, Square moveFrom, Piece moved){

	//	Piece piece = AI.GetInstance ().selectedPiece;

		//set default values
		pieces = (Piece[,])baseBoard.pieces.Clone();
		pieceList = new List<Piece> ();
		sideTable = baseBoard.sideTable;
		eval = 0;

		Debug.Log ("Created new board with piece: " + moved.name + "and moveTo: " + moveTo.x + ", " + moveTo.y);

		//adjust board and evaluate

		if (moved.isCaptured) {
			DropPiece (moveTo, moved);
		} else {
			Debug.Log ("Before move, " + moveTo.x + ", " + moveTo.y + " is: " + this.pieces[moveTo.x, moveTo.y]);
			MovePiece (moveTo, moveFrom, moved);
			Debug.Log ("After move, " + moveTo.x + ", " + moveTo.y + " is: " + this.pieces[moveTo.x, moveTo.y]);
		}

		GetPieceList ();
		this.eval = Evaluate ();

	}

	public void GetPieceList(){

		for (int i = 0; i < 9; i++){
			for (int j = 0; j < 9; j++){

				if (pieces [i, j] != null) {
					this.pieceList.Add (pieces [i, j]);
				}

			}
		}

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

	public List <Square> GetAllLegalMoves(bool isPlayerOne){

		List <Square> allMoves = new List<Square> ();

		if (isPlayerOne) { // is minimizing

			foreach (GameObject piece in AI.GetInstance().allPieces) {

				if (piece.GetComponent<Piece>().isPlayerOne) {

					List <Square> legalMoves = new List<Square> ();

					if (!piece.GetComponent<Piece>().isCaptured) {
						legalMoves = Square.coordsToMovesList (piece.GetComponent<Piece>().LegalMoves ());
					} else {
						legalMoves = Square.coordsToMovesList (AI.GetInstance().GetLegalDrops (piece.GetComponent<Piece>()));
					}

					foreach (Square move in legalMoves) {

						//caches the piece that was doing the moving since moves are recorded by square, not piece
						move.piecesMoving.Add (piece.gameObject);

						if (!allMoves.Contains (move)) {

							allMoves.Add (move);

						}
					}
				}
			}

		} else { // is maximizing

			foreach (GameObject piece in AI.GetInstance().allPieces) {

				if (!piece.GetComponent<Piece>().isPlayerOne) {

					List <Square> legalMoves = new List<Square> ();
					legalMoves = Square.coordsToMovesList (piece.GetComponent<Piece>().LegalMoves ());

					foreach (Square move in legalMoves){

						move.piecesMoving.Add (piece.gameObject);

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
