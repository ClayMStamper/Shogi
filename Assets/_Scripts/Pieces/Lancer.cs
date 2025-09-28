using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : Piece {

	public override bool[,] LegalMoves (){
		
		// possible moves for the player to make are added to this
		// array with index mapped with respect to coordinates
		//"moves" is the return variable
		bool[,] moves = new bool[9, 9];

		if (isPlayerOne) {
			
			// add orthagonal - down moves until a blocker is found
			for (int i = 1; i < 9; i++) {

				if (!PosIsBlocked (x, y - i)) {

					//move at x,y is legal
					moves [x, y - i] = true;

				} else { // is blocked
					
					if (y - i <= 8 && y - i >= 0) { //move is within the board
						
						//if blocker is an enemy
						if (BoardManager.GetInstance ().pieces [x, y - i].isPlayerOne != isPlayerOne) {
							
							// add one more move for attacking
							moves [x, y - i] = true;

						}
					}

					break;
				}
			}
		} else { //is player two: moves should be inverted

			// add orthagonal - up moves until a blocker is found
			for (int i = 1; i < 9; i++) {
				
				if (!PosIsBlocked (x, y + i)) {

					// move at x,y is legal
					moves [x, y + i] = true;

				} else {// is blocked
					
					if (y + i <= 8 && y + i >= 0) { //move is within the board
						
						//if blocker is an enemy
						if (BoardManager.GetInstance ().pieces [x, y + i].isPlayerOne != isPlayerOne) {
							
							// add one more move for attacking
							moves [x, y + i] = true;

						}
					}

					break;
				}
			}
		}

		return moves;

	}

}
