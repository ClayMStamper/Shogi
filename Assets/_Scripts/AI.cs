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

	public bool isActive = true;

	[Range (0, 100)]
	public int goodEnoughEval;

	[SerializeField]
	BoardManager boardManager;

	public List <GameObject> allPieces;

	[HideInInspector]
	public Piece selectedPiece = null;
	Square selectedMove;

//	int maxEval, minEval;                 

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
		boardManager.legalMoves = selectedPiece.LegalMoves (new Board (boardManager.pieces));

		if (!boardManager.legalMoves [selectedMove.x, selectedMove.y]) {
			Debug.LogError ("Fatal: AI is about to make an illegal move");
			Debug.LogError ("The piece: " + selectedPiece + " should not be able to move to " + selectedMove.x + ", " + selectedMove.y);
		}

		if (!selectedPiece.isCaptured) { //moving piece
			boardManager.legalMoves = selectedPiece.LegalMoves (new Board (boardManager.pieces));
			boardManager.MovePiece (selectedMove);
		} else { //dropping piece
			boardManager.legalMoves = GetLegalDrops (selectedPiece);
			boardManager.table1.DropPiece (selectedMove.x, selectedMove.y);
		}

	}

	//List <Square> rootMoves = new List<Square> ();
	Dictionary <Square, int> rootValues = new Dictionary<Square, int>();

	int Minimax(Board board, int depth, bool maximizing, int alpha, int beta){

//		Debug.Log ("Running minimax");

		if (depth == 0) {
			return board.eval;
		}
			
		List <Square> allSquaresToMoveTo = new List<Square> ();


		//is player 1s turn
		if (!maximizing) {

			int minEval = int.MaxValue;

			allSquaresToMoveTo = board.playerOneMoves;

			foreach (Square squareToMoveTo in allSquaresToMoveTo) {
				
				//the square that the piece hypothetically moves from
				Square moveFrom = new Square (new Vector2Int (squareToMoveTo.pieceMoving.x, squareToMoveTo.pieceMoving.y));
				Board newBoard = new Board (board, squareToMoveTo, moveFrom, squareToMoveTo.pieceMoving);


				int eval = Minimax (newBoard, depth - 1, true, alpha, beta);

				if (depth == recursionDepth) {
					rootValues.Add (squareToMoveTo, eval);
				}

				minEval = Mathf.Min (eval, minEval);

				if (eval < beta) { //found the better move

					SelectPieceAndSquare (squareToMoveTo);

					beta = minEval;
					Debug.Log ("New piece: " + selectedPiece.name + " should be selected with eval = " + beta);

					if (beta <= alpha) {
						return board.eval;
					}
				} else if (eval == beta) { //increase randomness in AI
					
					if ((selectedPiece is Pawn) && (squareToMoveTo.pieceMoving is Pawn)) {
						if (Random.value < 0.5f) {
							SelectPieceAndSquare (squareToMoveTo);
						}
					} else if (!(selectedPiece is Pawn) && (squareToMoveTo.pieceMoving is Pawn)) {
						SelectPieceAndSquare (squareToMoveTo);
					} 
				}


			}

			return minEval;

		} else {

			int maxEval = int.MinValue;

			allSquaresToMoveTo = board.GetAllLegalMoves (false);

			foreach (Square squareToMoveTo in allSquaresToMoveTo) {

				Square moveFrom = new Square (new Vector2Int (squareToMoveTo.pieceMoving.x, squareToMoveTo.pieceMoving.y));
				Board newBoard = new Board (board, squareToMoveTo, moveFrom, squareToMoveTo.pieceMoving);

				int eval = Minimax (newBoard, depth - 1, false, alpha, beta);
		//		maxEval = Mathf.Max (eval, maxEval);

				if (eval > maxEval) { //found the better move
					maxEval = eval;
					alpha = maxEval;
					if (beta <= alpha) {
						return board.eval;
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

	void SelectPieceAndSquare(Square square){
		selectedMove = square;
		selectedPiece = square.pieceMoving;
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

	public Square BestMove (){

		Square bestMove = rootValues.Keys.ToArray () [0];

		for (int i = 1; i < rootValues.Count; i++) {
			
			Square square = rootValues.Keys.ToArray () [i];

			if (rootValues [square] > rootValues [bestMove]) {
				bestMove = square;
			}

		}

		if (bestMove == null) {
			Debug.LogError ("Returning null");
		}

		return bestMove;

	}

}

// Abstraction of any coordinate on the board
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
	
public class Root : Square {


}

public struct Board{

	public int eval;
	public Piece[,] pieces;
	public List <Square> playerOneMoves;
	public List <Square> playerTwoMoves;
	public SideTable sideTable;

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

			//check every piece
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
