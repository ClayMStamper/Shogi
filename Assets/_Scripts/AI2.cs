using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void Go(){
		
		Piece bestPiece = pickPiece ();

		DoMove (selectedMove);

	}

	Piece pickPiece(){

		Piece piece = null;

		//replace this: it's random
		do {
			piece = allPieces[Random.Range (0, allPieces.Count)].GetComponent <Piece>();
		} while (!piece.isPlayerOne && piece != null);

		return piece;

	}

	Square calculateSquare(){

		Square bestMove = new Square ();

		return bestMove;

	}

	void DoMove(Square moveTo){

		BoardManager boardManager = BoardManager.GetInstance ();
		Piece piece = null;

		List <Square> legalMoves = new List<Square> ();
		legalMoves = Square.coordsToMovesList (piece.LegalMoves ());

		boardManager.SelectPiece (selectedPiece);

	}

	void Minimax(){

	}

	void CreateBoard(){

	}

}

struct Board{

	public Piece[,] pieces;

}

//"Move" nown: an available spot ont the board to move to
public struct Square{

	public int eval;
	public int x, y;

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

	public void Print(){
		Debug.Log ("available move: " + x + ", " + y);
	}

}
