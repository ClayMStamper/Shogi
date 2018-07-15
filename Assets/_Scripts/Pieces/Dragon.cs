using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Piece {

	// possible moves for the player to make are added to this
	// array with index mapped with respect to coordinates
	//"moves" is the return variable
	bool[,] moves = new bool[9, 9];

	Board board;

	public override bool[,] LegalMoves (Board board){

		this.board = board;

		moves = new bool[9, 9];

		// add orthagonal - down moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x, y - i)) {

				//move at x,y is legal
				moves [x, y - i] = true;

			} else { // is blocked

				if (y - i <= 8 && y - i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x, y - i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x, y - i] = true;

					}
				}

				break;
			}

		}

		// add orthagonal - up moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x, y + i)) {

				//move at x,y is legal
				moves [x, y + i] = true;

			} else { // is blocked

				if (y + i <= 8 && y + i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x, y + i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x, y + i] = true;

					}
				}

				break;
			}

		}

		// add orthagonal - right moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x + i, y)) {

				//move at x,y is legal
				moves [x + i, y ] = true;

			} else { // is blocked

				if (x + i <= 8 && x + i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x + i, y].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x + i, y] = true;

					}
				}

				break;
			}

		}

		// add orthagonal - right moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x - i, y)) {

				//move at x,y is legal
				moves [x - i, y ] = true;

			} else { // is blocked

				if (x - i <= 8 && x - i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x - i, y].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x - i, y] = true;

					}
				}

				break;
			}

		}

		SetMoveLegal (-1, 1);
		SetMoveLegal (0, 1);
		SetMoveLegal (1, 1);
		SetMoveLegal (1, 0);
		SetMoveLegal (1, -1);
		SetMoveLegal (0, -1);
		SetMoveLegal (-1, -1);
		SetMoveLegal (-1, 0);

		return moves;

	}

	//for adjacent moves only
	void SetMoveLegal(int xOffset, int yOffset){

		//piece is blocked by the edge of the board
		if (x + xOffset > 8 || x + xOffset < 0 || y + yOffset > 8 || y + yOffset < 0) {
			return;
		}

		if (!PosIsBlocked (board, x + xOffset, y + yOffset)) {
			moves [x + xOffset, y + yOffset] = true;
		} else { //is blocked
			//if blocker is an enemy
			if (board.pieces [x + xOffset, y + yOffset].isPlayerOne != isPlayerOne) {
				// add one more move to kill enemy/blocker
				moves [x + xOffset, y + yOffset] = true;

			}
		}
	}

}
