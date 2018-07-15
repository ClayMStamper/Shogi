﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Piece {

	bool[,] moves;
	Board board;

	public override bool[,] LegalMoves (Board board){

		this.board = board;

		moves = new bool[9, 9];

		if (isPlayerOne) {
			SetMoveLegal (0, 1);
			SetMoveLegal (1, 0);
			SetMoveLegal (1, -1);
			SetMoveLegal (0, -1);
			SetMoveLegal (-1, -1);
			SetMoveLegal (-1, 0);
		} else {
			SetMoveLegal (-1, 1);
			SetMoveLegal (0, 1);
			SetMoveLegal (1, 1);
			SetMoveLegal (1, 0);
			SetMoveLegal (0, -1);
			SetMoveLegal (-1, 0);
		}

		return moves;

	}

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
