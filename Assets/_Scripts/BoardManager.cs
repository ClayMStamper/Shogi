using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//On the "Board" object
[RequireComponent (typeof (AudioSource))]
public class BoardManager : MonoBehaviour {

	#region singleton

	//ensure that there is only one of these in the scene

	private static BoardManager instance;

	void Awake(){

		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

	//	DontDestroyOnLoad (gameObject);

	}

	public static BoardManager GetInstance(){
		return instance;
	}

	#endregion

	//the points that your mouse / finger are hovered over
	// defaulted to negative to indicate not being on the board
	public int selectedX= -1, selectedY = -1;

	[HideInInspector]
	public Client client;

	[HideInInspector]
	public Piece selectedPiece;
	[HideInInspector] 
	public List <Piece> activePieces;
	[SerializeField]
	AudioClip clickSound;
	[SerializeField]
	GameObject gameOverPopup;

	public Piece[,] pieces;

	[HideInInspector]
	public Board currentBoard;

	[HideInInspector]
	public bool isPlayerOnesTurn = false;

	public bool[,] legalMoves;

	List <Piece> table1Pieces, table2Pieces;
	public SideTableManager table1, table2;

	//cached HighlightManager reference
	HighlightManager highlightManager;

	void Start(){
		highlightManager = HighlightManager.GetInstance ();
		client = FindObjectOfType <Client> ();
		this.pieces = new Piece[9, 9];
		SetUpBoard ();
	}

	void Update(){
		
		UpdateSelection ();
		OnClick();

	}
		
	void SetUpBoard(){

		table1Pieces = new List<Piece> ();
		table2Pieces = new List<Piece> ();

		//initialize piece collections
		activePieces = FindObjectsOfType <Piece> ().ToList();

		//set up piece references
		foreach (Piece piece in activePieces) {
			//map 2d array coordinates to piece coordinates
			pieces [piece.x, piece.y] = piece;
		}
			
		currentBoard = new Board (pieces);

		//currently always starts as player 2's turn (not AI's)
		isPlayerOnesTurn = Random.value < 0.5f ?  false : false;
			

	}

	//keeps track of what piece you have selected
	void UpdateSelection(){
		
		if (Camera.main == null)
			return;

		RaycastHit hit;
		//board is selected
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition),
			out hit,
			25f,
			LayerMask.GetMask ("Board"))) {

			selectedX = (int)hit.point.x;
			selectedY = (int)hit.point.z;

			//not being selected
		}	else {
			selectedX = -1;
			selectedY = -1;
		}
			

	}

	void OnClick(){
		
		if (Input.GetMouseButtonDown (0)) {

			//is selecting on the board
			//if (selectedX >= 0 && selectedY >= 0) {

			if (selectedX >= 0 && selectedY >= 0) {
				// piece is not already selected
				if (selectedPiece == null) {

					SelectPiece (selectedX, selectedY);

					// piece is selected
					// move or drop piece
				} else {				

					//piece is on the board: move it
					if (!selectedPiece.isCaptured) {
						MovePiece ();
					} else {
						//piece is on side table: drop it
						if (selectedPiece.isPlayerOne) {
							table1.DropPiece (selectedX, selectedY);
						} else {
							table2.DropPiece (selectedX, selectedY);
						}
					}
				}

			}
		}

	}

	//sets var "selectedPiece" to piece at clicked on coord
	//this happens if selectedPiece == null
	public void SelectPiece (int x, int y){

		if (client != null) {

			bool isPlayerOne = client.isHost;

			if (isPlayerOne != isPlayerOnesTurn) {
				return;
			} 

		}

		//selected coord is empty
		if (pieces [x, y] == null) {
			Debug.Log ("Slected Piece is null");
			return;
		}
		//return if not my piece
		if (pieces [x, y].isPlayerOne != isPlayerOnesTurn) {
			return;
		}

		legalMoves = pieces [x, y].LegalMoves (new Board (pieces));
		selectedPiece = pieces [x, y];
	//	SideTableManager.selectedPiece = selectedPiece;
		if (Settings.GetInstance ().highlightMoves) {
			highlightManager.ShowLegalMoves (legalMoves);
		}
		bool[,] mySquare = new bool[9, 9];
		mySquare [x, y] = true;
		highlightManager.ShowLegalMoves (mySquare);

	}

	public void SelectPiece(Piece piece){

		legalMoves = piece.LegalMoves (new Board (pieces));
		selectedPiece = piece;
		//	SideTableManager.selectedPiece = selectedPiece;
		if (Settings.GetInstance ().highlightMoves) {
			highlightManager.ShowLegalMoves (legalMoves);
		} 
		bool[,] mySquare = new bool[9, 9];
		mySquare [piece.x, piece.y] = true;
		highlightManager.ShowLegalMoves (mySquare);

	}

	//Moves slectedPiece variable to passed in coord
	//this happens if selectedPiece != null
	public void MovePiece (){

		//check for move is legal
		if (legalMoves [selectedX, selectedY]) {

			if (client != null && client.isHost == isPlayerOnesTurn) {
				NetworkedMovePiece (selectedX, selectedY);
				return;
			}

			//if (MovedIntoCheck ()) { //move will result in a loss
			//	Debug.Log ("MOVING INTO CHECK!!!");
			//} else { //perform move

				//enemy piece is here
				if (pieces [selectedX, selectedY] != null) {
					OnPieceWasCaptured (pieces [selectedX, selectedY]);
				}
				
				OnPieceWasMoved ();

		//	}
		} else {
			if (AI.GetInstance ().isActive && isPlayerOnesTurn) {
				//Debug.LogError ("Fatal: AI made an illegal move: " + selectedPiece + ": " + selectedX + ", " + selectedY);
			}
			Debug.Log ("ILLEGAL MOVE");
		}

		//un-select piece
		selectedPiece = null;
		highlightManager.HideMoves ();

	}

	void NetworkedMovePiece(int x, int y){

		Debug.Log ("Networked move piece");

		//enemy piece is here
		if (pieces [x, y] != null) {
			OnPieceWasCaptured (pieces [x, y]);
		}

		string msg = "CMOV|";
		msg += selectedPiece.x.ToString () + "|";
		msg += selectedPiece.y.ToString () + "|";
		msg += x.ToString () + "|";
		msg += y.ToString () + "|";

		client.Send (msg);

		OnPieceWasMoved ();

		//un-select piece
		selectedPiece = null;
		highlightManager.HideMoves ();

	}

	public void MovePiece(Square move){

		selectedX = move.x;
		selectedY = move.y;

	//	if (MovedIntoCheck ()) {
	//		Debug.Break ();
	//	}

		//enemy piece is here
		if (pieces[move.x, move.y] != null){
			Debug.Log ("Trying to capture");
			OnPieceWasCaptured(pieces [move.x, move.y]);
		}

		OnPieceWasMoved ();

		//un-select piece
		selectedPiece = null;
		highlightManager.HideMoves ();

	}

	bool MovedIntoCheck(){
		
		Square moveTo = new Square (new Vector2Int (selectedX, selectedY));
		Square moveFrom = new Square (new Vector2Int (selectedPiece.x, selectedPiece.y));
		Board baseBoard = new Board (pieces);

		Board newBoard = new Board (baseBoard, moveTo, moveFrom, selectedPiece);

		//bool[,] oppMoves = new bool[9, 9];
		List <Square> oppMoves = newBoard.GetAllLegalMoves (!(selectedPiece.isPlayerOne == isPlayerOnesTurn));
		List <Square> myMoves = newBoard.GetAllLegalMoves ((selectedPiece.isPlayerOne == isPlayerOnesTurn));

		foreach (Square move in oppMoves) {
			move.Print ();
		}

		foreach (Square move in myMoves) {
			move.Print ();
		}


		King myKing = FindObjectOfType <King>();

		foreach (King king in FindObjectsOfType <King>()){
			if (king.isPlayerOne == isPlayerOnesTurn) {
				myKing = king;
			}
		}

		foreach (Square move in oppMoves) {

			if (myKing.x == move.x && myKing.y == move.y) {

				Debug.Log ("===FOUND CHECK MOVE===");
				//moveTo.Print ();
				//moveFrom.Print ();

				Debug.Log (move.pieceMoving  + " can take my king, by moving to sqaure: " + move.x + ", " + move.y);
				move.Print ();

				return true;
			}

		}

		return false;

	}

	void OnPieceWasMoved(){

		if (Settings.GetInstance ().soundIsOn) {
			GetComponent <AudioSource> ().clip = clickSound;
			GetComponent <AudioSource> ().Play ();
		}

		CheckForPromotion ();

		//set old pos to null 
		pieces [selectedPiece.x, selectedPiece.y] = null;
		//set newPos to selected coordinate
		pieces [selectedX, selectedY] = selectedPiece;
		//.5 compensation for truncated floats to ints 
		selectedPiece.transform.position = new Vector3 (selectedX + .5f, selectedPiece.transform.position.y, selectedY + .5f);
		//cache new position
		selectedPiece.GetCurrentPos ();

		//Debug.Log ("Y pos: " + selectedPiece.y);

		CheckForPromotion ();

		if (Settings.GetInstance ().showScore) {
			try{
				Score.GetInstance ().UpdateScore ();
			} catch {
				Debug.LogError ("Trying to update the score while disabled");
			}
		}

		//switch turns
		isPlayerOnesTurn = !isPlayerOnesTurn;

		if (AI.GetInstance().isActive && isPlayerOnesTurn) {
			AI.GetInstance ().Go ();
		}

	}

	// call SideTableManager Functions that
	// physically move piece to side table
	void OnPieceWasCaptured (Piece piece){

		//king was captured
		if (piece.GetComponent<King> ()) {
			gameOverPopup.SetActive (true);
		}

		//any captured piece is demoted if it was promoted

		pieces [selectedX, selectedY] = piece;

		GameObject table;

		//child the piece to its table, respectively, to use local pos
		if (piece.isPlayerOne) {

			//piece goes to player 2
			piece = Demote (piece.gameObject);
			table2Pieces.Add (piece);
			table = GameObject.Find ("Table 2");

			piece.isPlayerOne = false;
	
		} else {

			//piece goes to player 1
			piece = Demote (piece.gameObject);
			table1Pieces.Add (piece);
			table = GameObject.Find ("Table 1");

			piece.isPlayerOne = true;

		}

		piece.isCaptured = true;

		if (table != null) {

			Debug.Log ("Adding " + piece + " to " + table);

			//add to the side table
			table.GetComponent<SideTableManager> ().AddPiece (piece);

		} else {
			
			Debug.LogError ("Could not find side-table: make sure objects are correctly named");

		}

	}

	void CheckForPromotion(){

		//if not already promoted and is promotable
		if (!selectedPiece.GetComponent <GoldPromo> () &&
			!selectedPiece.GetComponent <Gryphon>() &&
			!selectedPiece.GetComponent <Dragon>() &&
			!selectedPiece.GetComponent <Gold>() &&
			!selectedPiece.GetComponent <King>()) {
			if (selectedPiece.isPlayerOne) {

				if (selectedPiece.y <= 2) {
					selectedPiece = Promote (selectedPiece.gameObject);
					pieces [selectedX, selectedY] = selectedPiece;
					selectedPiece.isPlayerOne = true;
				}

			} else {

				if (selectedPiece.y >= 6) {
					selectedPiece = Promote (selectedPiece.gameObject);
					pieces [selectedX, selectedY] = selectedPiece;
				}

			}
		}

	}

	//Will always be a result of moving
	Piece Promote(GameObject pieceObj){

		//change piece script to gold
		Destroy (pieceObj.GetComponent<Piece> ());

		Piece promoType;

		if (pieceObj.name == "Bishop") {
			promoType = pieceObj.AddComponent<Gryphon> ();
		} else if (pieceObj.name == "Rook") {
			promoType = pieceObj.AddComponent<Dragon> ();
		} else { //is any other non-prmoted & promotable piece
			promoType = pieceObj.AddComponent<GoldPromo> ();
		}

		pieceObj.transform.Rotate(new Vector3 (180, 0 ,180));

		return promoType;

	}

	//will always be a result of being captured
	Piece Demote(GameObject pieceObj){

		Piece demoType = pieceObj.GetComponent <Piece>();

		//if is promoted
		if (pieceObj.GetComponent <GoldPromo> () ||
			pieceObj.GetComponent <Gryphon> () ||
			pieceObj.GetComponent <Dragon> ()) {

			Destroy (pieceObj.GetComponent <Piece> ());

			switch (pieceObj.name) {

			case "Pawn":
				Debug.Log ("Demoting a pawn");
				demoType = pieceObj.AddComponent <Pawn> ();
				break;
			case "Knight":
				demoType = pieceObj.AddComponent <Knight> ();
				break;
			case "Bishop":
				demoType = pieceObj.AddComponent <Bishop> ();
				break;
			case "Rook":
				demoType = pieceObj.AddComponent <Rook> ();
				break;
			case "Lancer":
				demoType = pieceObj.AddComponent <Lancer> ();
				break;
			case "Silver":
				demoType = pieceObj.AddComponent <Silver> ();
				break;
			case "Gold":
				demoType = pieceObj.AddComponent <Gold> ();
				break;
			default:
				Debug.LogError ("DEMOTION FAILED: Piece obj name not recognized");
				break;

			}

			pieceObj.transform.Rotate(new Vector3 (180, 0 ,180));
			//pieces [selectedX, selectedY] = demoType;

		}

		return demoType;

	}

}
