using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

	public override bool[,] LegalMoves (Board board){

		// possible moves for the player to make are added to this
		// array with index mapped with respect to coordinates
		//"moves" is the return variable
		bool[,] moves = new bool[9, 9];

		// add diagonal - down/left moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x - i, y - i)) {

				//move at x,y is legal
				moves [x - i, y - i] = true;

			} else { // is blocked

				if (y - i <= 8 && y - i >= 0 && x - i <= 8 && x - i >= 0) { //move is within the board

					try {
					//if blocker is an enemy
					if (board.pieces [x - i, y - i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x - i, y - i] = true;

					}
					} catch {
						Debug.LogError (name + " at " + x + ", " + y + " is looking at the board wrong at recursion depth: " + AI.GetInstance ().currentRecursionDepth);
					}
				}

				break;
			}

		}

		// add diagonal - down/right moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x + i, y - i)) {

				//move at x,y is legal
				moves [x + i, y - i] = true;

			} else { // is blocked

				if (y - i <= 8 && y - i >= 0 && x + i <= 8 && x + i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x + i, y - i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x + i, y - i] = true;

					}
				}

				break;
			}

		}

		// add diagonal - up/left moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x - i, y + i)) {

				//move at x,y is legal
				moves [x - i, y + i] = true;

			} else { // is blocked

				if (y + i <= 8 && y + i >= 0 && x - i <= 8 && x - i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x - i, y + i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x - i, y + i] = true;

					}
				}

				break;
			}

		}

		// add diagonal - up/right moves until a blocker is found
		for (int i = 1; i < 9; i++) {

			if (!PosIsBlocked (board, x + i, y + i)) {

				//move at x,y is legal
				moves [x + i, y + i] = true;

			} else { // is blocked

				if (y + i <= 8 && y + i >= 0 && x + i <= 8 && x + i >= 0) { //move is within the board

					//if blocker is an enemy
					if (board.pieces [x + i, y + i].isPlayerOne != isPlayerOne) {

						// add one more move for attacking
						moves [x + i, y + i] = true;

					}
				}

				break;
			}

		}

		return moves;

	}

}
