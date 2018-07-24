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

