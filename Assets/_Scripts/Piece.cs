using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	public bool isPlayerOne;
	public bool isCaptured;

	public int x { set; get;}
	public int y { set; get;}

	BoardManager boardManager;
	AI ai;

	[HideInInspector]
	public Animator anim;

	public bool[,] availableMoves;

	void Awake(){

		GetCurrentPos (); //references position on board as variable x & y

	}

	void Start(){
		boardManager = BoardManager.GetInstance ();
		ai = AI.GetInstance ();
		anim = GetComponent <Animator> ();
	}

	//get my current coord
	public void GetCurrentPos(){
		x = (int)transform.position.x;
		y = (int)transform.position.z;
	}

	public virtual bool[,] LegalMoves(Board board){
		return new bool[9,9];
	}

	public virtual List <Square> LegalMovesList (Board board){
		return coordsToMovesList (LegalMoves (board));
	}

	public bool[,] LegalDrops(Board board){

		bool[,] legalMoves = new bool[9, 9];

		if (name != "Pawn") { // not a pawn

			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 9; j++) {

					if (!PosIsBlocked (board, i, j)) {

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

					if (PosIsBlocked (board, i, j)) {

						legalMoves [i, j] = false;

						if (board.pieces [i, j] is Pawn && 
							board.pieces[i, j].isPlayerOne == isPlayerOne) {

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

	public List <Square> LegalDropsList(Board board){
		return coordsToMovesList (LegalDrops (board));
	}

	//check if passed in tile coord is blocked
	public bool PosIsBlocked(Board board, int x, int y){
		
		boardManager = BoardManager.GetInstance ();
		ai = AI.GetInstance ();

		//piece is blocked by the edge of the board
		if (x > 8 || x < 0 || y > 8 || y < 0) {
			return true;
		}
		if (board.pieces [x, y] != null) {
			//there is a piece on that tile
			if (board.pieces [x, y].isPlayerOne == this.isPlayerOne) {
				//it's a friendly piece
				return true;
			} else {
				//it's an enemy piece
				return true;
			}

		}
		// no piece on that tile
		return false;
	}

	//converts vector array to Move array
	private List<Square> coordsToMovesList(bool[,] moveIsLegalArr){

		List <Square> newMovesList = new List<Square> ();

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {

				if (moveIsLegalArr [i, j]) {
					newMovesList.Add (new Square (new Vector2Int (i, j), this));
				}

			}
		}

		return newMovesList;

	}

}
