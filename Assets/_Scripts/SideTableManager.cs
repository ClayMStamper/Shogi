using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * This script just references the BoardManager to assign the boards
 * "selectedPiece" and "legalMoves" if the side table is selected
 */

public class SideTableManager : MonoBehaviour {

	//the points that your mouse / finger are hovered over
	// defaulted to negative to indicate not being on the board
	public int selectedX = -1, selectedY = -1;

	//public static Piece selectedPiece;

	public Stack <Piece>[,] pieceStacks;

	//The grid is an empty transform scaled so that 
	//pieces can be childed to it with an approptiate
	//local, relative scale
	[SerializeField]
	Transform grid;

	[SerializeField]
	BoardManager boardManager;

	void Awake (){

		pieceStacks = new Stack<Piece>[3,3];

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				pieceStacks [i, j] = new Stack<Piece> ();
			}
		}

		boardManager.legalMoves = new bool[9,9];

	}

	void Update(){

		//return if its not my turn
		if (boardManager.isPlayerOnesTurn != name.Contains ("1")) {
			return;
		}

		UpdateSelection ();
		OnClick ();

	}

	public void AddPiece(Piece piece){

		piece.transform.SetParent(grid);

		Vector3Int coord = new Vector3Int ();

		// the tables parents are rotated so Z & Y are 
		// effectively swapped
		switch (piece.name) {
		case "Pawn":
			coord = new Vector3Int (0, 2, 0);
			break;
		case "Lancer":
			coord = new Vector3Int (1, 2, 0);
			break;
		case "Knight":
			coord = new Vector3Int (2, 2, 0);
			break;
		case "Silver":
			coord = new Vector3Int (0, 1, 0);
			break;
		case "Gold":
			coord = new Vector3Int (1, 1, 0);
			break;
		case "Bishop":
			coord = new Vector3Int (2, 1, 0);
			break;
		case "Rook":
			coord = new Vector3Int (0, 0, 0);
			break;
		default:
			Debug.LogError ("Piece name: " + piece.name + ", not recognized: failed to add to 'pieces'");
			break;
		}

		//physically move piece
		piece.transform.localPosition = (Vector3)coord;

		//add to stack of pieces
		pieceStacks [coord.x, coord.y].Push (piece);
		//Debug.Log (piece + " was added to stack: " + coord.x + " ," + coord.y);
		//Debug.Log (pieceStacks [coord.x, coord.y].Peek ());
		OnWasStacked (piece, pieceStacks [coord.x, coord.y]);

		//rotate because piece is swapping team
		piece.transform.Rotate (new Vector3 (0, 0, 180));

	}

	void OnWasStacked(Piece piece, Stack<Piece> stack){
		//translate piece up to physically stack repeted pieces
		Vector3 newPos = piece.transform.localPosition;
		float zTranslation = (stack.Count - 1) * 0.1f;
		float yTranslation = (stack.Count - 1) * 0.01f;
		newPos.z -= zTranslation; //negative because axis is inverted
		newPos.y -= yTranslation;
		piece.transform.localPosition = newPos;
	}

	void UpdateSelection(){

		if (Camera.main == null)
			return;

		RaycastHit hit;
		//board is selected
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition),
			out hit,
			25f,
			LayerMask.GetMask ("Side Table"))) {

			Vector3 selection = new Vector3 (hit.point.x, 0, hit.point.z);
			selection = grid.InverseTransformPoint (selection);

			selectedX = Mathf.RoundToInt(selection.x);
			selectedY = Mathf.RoundToInt(selection.y);

			//not being selected
		}	else {
			selectedX = -1;
			selectedY = -1;
		}

	}

	void OnClick(){

		if (Input.GetMouseButtonDown (0)) {

			// select piece
			if (boardManager.selectedPiece == null) {
				
				if (selectedX >= 0 && selectedY >= 0) {

					SelectPiece ();
			
				} 
			} 
		}
	}

	//sets selectedPiece & legelMoves in BoardManager
	public void SelectPiece (){

		//Debug.Log ("Selecting piece at " + selectedX + ", " + selectedY);

		//selected coord is empty
		if (pieceStacks [selectedX, selectedY].Peek() == null) {
			Debug.Log ("Slected Piece is null");
			return;
		}
		//return if not my piece
		if (pieceStacks [selectedX, selectedY].Peek().isPlayerOne != boardManager.isPlayerOnesTurn) {
			Debug.Log ("Not my piece");
			//return;
		}
			
		//selected piece = top of stack at x, y
		boardManager.selectedPiece = pieceStacks [selectedX, selectedY].Peek();

		GetLegalMoves ();
		HighlightManager.GetInstance().ShowLegalMoves (boardManager.legalMoves);

		//Debug.Log ("Piece at: " + selectedX + ", " + selectedY + " = ");
		//Debug.Log (pieceStacks [selectedX, selectedY].Peek ());

		//Debug.Log (board.selectedPiece);

	}

	public void DropPiece (int x, int y){

		//check for move is legal
		if (boardManager.legalMoves [x, y]) {

			Debug.Log ("SHOULD BE SENDING DROP TO SERVER");

			Vector3 coord = boardManager.selectedPiece.transform.localPosition;

			if (boardManager.client != null && boardManager.client.isHost == boardManager.isPlayerOnesTurn) {

				string msg = "CDROP|";

				msg += coord.x.ToString () + "|";
				msg += coord.y.ToString () + "|";
				msg += x.ToString () + "|";
				msg += y.ToString () + "|";

				boardManager.client.Send (msg);

			}
					
			pieceStacks [Mathf.RoundToInt(coord.x) , Mathf.RoundToInt(coord.y)].Pop();

			//set newPos to selected coordinate
			boardManager.pieces [x, y] = boardManager.selectedPiece;
			//.5 compensation for truncated floats to ints 
			boardManager.selectedPiece.transform.position = new Vector3 (x + .5f, 0, y + .5f);
			//cache new position
			boardManager.selectedPiece.GetCurrentPos ();
			//parent piece to the board object
			boardManager.selectedPiece.transform.SetParent (boardManager.transform);

			//switch turns
			boardManager.isPlayerOnesTurn = !boardManager.isPlayerOnesTurn;

			//organize piece gameobject
			if (boardManager.selectedPiece.isPlayerOne) {
				boardManager.selectedPiece.transform.SetParent (GameObject.Find ("Player 1").transform);
			} else {
				boardManager.selectedPiece.transform.SetParent (GameObject.Find ("Player 2").transform);
			}

			boardManager.selectedPiece.isCaptured = false;		

			boardManager.GetComponent<AudioSource> ().Play ();

			//update score
			if (Settings.GetInstance ().showScore) {
				try{
					Score.GetInstance ().UpdateScore ();
				} catch {
					Debug.LogError ("Trying to update the score while disabled");
				}
			}

		} else {
			Debug.Log ("ILLEGAL DROP");
		}

		//un-select piece
		boardManager.selectedPiece = null;
		HighlightManager.GetInstance().HideMoves ();

		//AI Go
		if (AI.GetInstance().isActive && boardManager.isPlayerOnesTurn) {
			AI.GetInstance ().Go ();
		}

	}

	//all open spots are legal drops with the exception of pawns
	void GetLegalMoves(){

		if (boardManager.selectedPiece.name != "Pawn") { // not a pawn
			
			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 9; j++) {
				
					if (!PosIsBlocked (i, j)) {
					
						boardManager.legalMoves [i, j] = true;

						//Debug.Log (i + ", " + j + ": Is Moveable");

					} else {

						boardManager.legalMoves [i, j] = false;

					}
				}
			}

		} else { //is a pawn: apply special rules
			
			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 9; j++) {

					if (PosIsBlocked (i, j)) {

						boardManager.legalMoves [i, j] = false;

						if (boardManager.pieces [i, j] is Pawn && 
						boardManager.pieces[i, j].isPlayerOne == boardManager.selectedPiece.isPlayerOne) {

							for (int k = 0; k < 9; k++) {

								//sets moves in the file to illegal if a friendly
								//pawn is in that file
								boardManager.legalMoves [i, k] = false;

							}

							break;

						}

					} else {

						boardManager.legalMoves [i, j] = true;

					}
				}
			}
		}
	}

	bool PosIsBlocked (int x, int y){

		if (boardManager.pieces [x, y] != null) {
			return true;
		} else {
			return false;
		}

	}

}
