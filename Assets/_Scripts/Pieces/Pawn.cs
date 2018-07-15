using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

	public override bool[,] LegalMoves (Board board)
	{
		// possible moves for the player to make are added to this
		// array with index mapped with respect to coordinates
		//"moves" is the return variable
		bool[,] moves = new bool[9, 9];

		if (isPlayerOne) {
			
			if (!PosIsBlocked (board, x, y - 1)) {
				
				moves [x, y - 1] = true;

			} else { // is blocked
				
				if (y - 1  <= 8 && y - 1 >= 0) { //move is on board
					
					//if blocker is an enemy
					if (board.pieces [x, y - 1].isPlayerOne != isPlayerOne) {
						
						// add one more move to kill enemy/blocker
						moves [x, y - 1] = true;

					}
				}

			}
		} else { // is player two
			
			if (!PosIsBlocked (board, x, y + 1)) {
				
				moves [x, y + 1] = true;

			} else { // is blocked

				if (y + 1 <= 8 && y + 1 >= 0) { //move is on board
					
					//if blocker is an enemy
					if (board.pieces [x, y + 1].isPlayerOne != isPlayerOne) {
						
						// add one more move to kill enemy/blocker
						moves [x, y + 1] = true;

					}
				}

			} 
		}

		return moves;

	}

}
