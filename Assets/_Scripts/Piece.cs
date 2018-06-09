using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {

	public bool isPlayerOne;
	public bool isCaptured;

	public int x { set; get;}
	public int y { set; get;}

	BoardManager board;

	[HideInInspector]
	public Animator anim;

	public bool[,] availableMoves;

	void Awake(){

		GetCurrentPos (); //references position on board as variable x & y

	}

	void Start(){
		board = BoardManager.GetInstance ();
		anim = GetComponent <Animator> ();
	}

	//get my current coord
	public void GetCurrentPos(){
		x = (int)transform.position.x;
		y = (int)transform.position.z;
	}

	public virtual bool[,] LegalMoves(){
		return new bool[9,9];
	}

	//check if passed in tile coord is blocked
	public bool PosIsBlocked(int x, int y){

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

}
