using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {

	public override bool[,] LegalMoves (Board board){

		// possible moves for the player to make are added to this
		// array with index mapped with respect to coordinates
		//"moves" is the return variable
		bool[,] moves = new bool[9, 9];

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

		return moves;

	}

}
