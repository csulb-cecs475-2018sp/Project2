using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class KnightTests : ChessTest {
		//Checks the knight moves for black pieces
		[Fact]
		public void KnightMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4");
			var moves = b.GetPossibleMoves();
			var expectedMoves = GetMovesAtPosition(moves, Pos("b8"));
			expectedMoves.Should().HaveCount(2, "The knight at location B8 should be able to move to A6 and C6")
				 .And.Contain(Move("b8,a6"))
				 .And.Contain(Move("b8,c6"));
		}

		[Fact] //w
		public void UndoKnightMoveTwice() {
			ChessBoard b = CreateBoardFromMoves(
				 "g1, f3",
				 "f7, f5",
				 "f3, d4"
			);

			b.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Knight, "Knight moved to d4");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("f3")).PieceType.Should().Be(ChessPieceType.Knight, "Knight returns back to f3");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("f7")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn is at initial position");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Knight is at initial position");

		}

		/// <summary>
		/// Use white knight to validate that GetPossibleMoves correctly reports all possible moves
		/// </summary>
		[Fact]
		public void KnightValidatePossibleMoves() {
			ChessBoard b = new ChessBoard();

			var possMoves = b.GetPossibleMoves();
			b.CurrentPlayer.Should().Be(1, "white's turn");

			var twoMovesExpected = GetMovesAtPosition(possMoves, Pos("b1"));
			twoMovesExpected.Should().Contain(Move("b1, a3"))
				 .And.Contain(Move("b1, c3"))
				 .And.HaveCount(2, "white knight starting at B1 can't move to D2 because it is occupied");


			Apply(b, "d2, d4",
				 "h7, h6"); // move black pawn to change turns
			possMoves = b.GetPossibleMoves(); // recalculate possible moves
			b.CurrentPlayer.Should().Be(1, "white's turn");

			var threeMovesExpected = GetMovesAtPosition(possMoves, Pos("b1"));
			threeMovesExpected.Should().Contain(Move("b1, a3"))
				 .And.Contain(Move("b1, c3"))
				 .And.Contain(Move("b1, d2"))
				 .And.HaveCount(3, "white knight starting at B1 can now move to D2");

			Apply(b, "b1, c3", // move white knight 
				 "h6, h5", // move pieces out of the way
				 "d1, d3",
				 "h5, h4",
				 "e2, e3",
				 "h4, h3",
				 "a2, a3",
				 "g7, g6");
			possMoves = b.GetPossibleMoves(); // recalculate possible moves
			b.CurrentPlayer.Should().Be(1, "white's turn");

			var eightMovesExpected = GetMovesAtPosition(possMoves, Pos("c3"));
			var count = eightMovesExpected.Count();
			eightMovesExpected.Should().Contain(Move("c3, b1"))
				 .And.Contain(Move("c3, a2"))
				 .And.Contain(Move("c3, a4"))
				 .And.Contain(Move("c3, b5"))
				 .And.Contain(Move("c3, d5"))
				 .And.Contain(Move("c3, e4"))
				 .And.Contain(Move("c3, e2"))
				 .And.Contain(Move("c3, d1"))
				 .And.HaveCount(8, "white knight has uninhibited movement");
		}

		/// <summary>
		/// At the start, the knights should be able to move to two positions jumping over the pawns.
		/// Test : - Initial Starting Board state
		/// Player: - Black
		/// Piece: - Knight
		/// Position: - b8
		/// Desired Positions: - a6, c6
		/// </summary>
		[Fact]
		public void ValidStartingMoveForKnight() {
			ChessBoard board = new ChessBoard();

			//Move a white knight so that it is black's turn
			Apply(board, "b1,a3");
			var possibleMoves = board.GetPossibleMoves();
			var initialKnightMoves = GetMovesAtPosition(possibleMoves, Pos("b8"));

			initialKnightMoves.Should().HaveCount(2, "The knight should be able to move to two different positions" +
				 "in front of the pawns");

		}

		/// <summary>
		/// At most 1 test can be about the initial starting board state. Done
		/// </summary>
		[Fact]
		public void checkTurnOneMovesForKnights() {
			ChessBoard b = CreateBoardFromMoves();

			var possMoves = b.GetPossibleMoves();
			var twoMovesExpected = GetMovesAtPosition(possMoves, Pos("b1"));
			twoMovesExpected.Should().Contain(Move("b1, a3"))
				 .And.Contain(Move("b1, c3"))
				 .And.HaveCount(2, "a knight should be able to move into two different cells on each side for their first turn");

			possMoves = b.GetPossibleMoves();
			twoMovesExpected = GetMovesAtPosition(possMoves, Pos("g1"));
			twoMovesExpected.Should().Contain(Move("g1, f3"))
				 .And.Contain(Move("g1, h3"))
				 .And.HaveCount(2, "a knight should be able to move into two different cells on each side for their first turn");

			Apply(b, Move("b1, a3"));

			possMoves = b.GetPossibleMoves();
			twoMovesExpected = GetMovesAtPosition(possMoves, Pos("b8"));
			twoMovesExpected.Should().Contain(Move("b8, a6"))
				 .And.Contain(Move("b8, c6"))
				 .And.HaveCount(2, "a knight should be able to move into two different cells on each side for their first turn");

			possMoves = b.GetPossibleMoves();
			twoMovesExpected = GetMovesAtPosition(possMoves, Pos("g8"));
			twoMovesExpected.Should().Contain(Move("g8, f6"))
				 .And.Contain(Move("g8, h6"))
				 .And.HaveCount(2, "a knight should be able to move into two different cells on each side for their first turn");
		}

		/// <summary>
		/// Checks valid Knight moves from board created.
		/// </summary>
		[Fact]
		public void CheckKnightMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "b2, b4",
				 "f7, f6",
				 "g1, f3",
				 "b8, a6",
				 "f3, e5",
				 "c7, c6",
				 "d2, d3",
				 "g7, g6"
			);

			var possMoves = b.GetPossibleMoves();
			var Player1KnightMoves = GetMovesAtPosition(possMoves, Pos("e5"));
			Player1KnightMoves.Should().HaveCount(7, "Knight has seven valid moves, one move blocked by friendly pawn");
		}

		/// <summary>
		/// Testing undo of knight capture
		/// </summary>
		[Fact]
		public void UndoKnightCapture() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"e7, e5",
				"c4, c5",
				"e5, e4",
				"c5, c6"
			);

			var possMoves = b.GetPossibleMoves();
			var kightCaptureExpected = GetMovesAtPosition(possMoves, Pos("b8"));
			kightCaptureExpected.Should().HaveCount(2, "kight can move two different ways from b8")
				.And.Contain(Move("b8, a6"))
				.And.Contain(Move(Pos("b8"), Pos("c6"), ChessMoveType.Normal));

			// Apply the knight capture passant
			Apply(b, Move(Pos("b8"), Pos("c6"), ChessMoveType.Normal));
			var knight = b.GetPieceAtPosition(Pos("c6"));
			knight.Player.Should().Be(2, "kight performed capture");
			knight.PieceType.Should().Be(ChessPieceType.Knight, "kight performed capture");
			b.CurrentAdvantage.Should().Be(Advantage(2, 1), "player 2 captured player 1's pawn with a knight");

			// Undo the move and check the board state
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "knight capture was undone");
			knight = b.GetPieceAtPosition(Pos("b8"));
			knight.Player.Should().Be(2, "knight capture was undone");
			var captured = b.GetPieceAtPosition(Pos("c6"));
			captured.Player.Should().Be(1, "knight capture was undone");
			var boardIntegrity = b.GetPieceAtPosition(Pos("e4"));
			boardIntegrity.Player.Should().Be(2, "player 2 has a piece at e4");
			boardIntegrity.PieceType.Should().Be(ChessPieceType.Pawn, "knight capture was undone");
		}

		[Fact]
		public void KnightStartingMoves() {
			ChessBoard b = new ChessBoard();

			var possMoves = b.GetPossibleMoves();

			// Checks possible moves for the white's left knight
			var leftWhiteKnight = GetMovesAtPosition(possMoves, Pos("b1"));
			leftWhiteKnight.Should().Contain(Move("b1, a3")).And.Contain(Move("b1, c3"))
			.And.HaveCount(2, "White's left knight should have two possible moves at the start");

			Apply(b, "a2, a3");

			// moves black pawn, blocking one of the possible moves of the left black knight
			Apply(b, "c7, c6");

			Apply(b, "a3, a4");
			possMoves = b.GetPossibleMoves();

			// Checks possible moves for the black's left knight
			var leftBlackKnight = GetMovesAtPosition(possMoves, Pos("b8"));
			leftBlackKnight.Should().Contain(Move("b8, a6")).
			And.HaveCount(1, "Black's left knight should have 1 possible moves after black pawn is moved");

			b.UndoLastMove();
			b.UndoLastMove();

			possMoves = b.GetPossibleMoves();

			// Checks possible moves for the black's left knight without a pawn at c6
			leftBlackKnight = GetMovesAtPosition(possMoves, Pos("b8"));
			leftBlackKnight.Should().Contain(Move("b8, a6")).And.Contain(Move("b8,c6")).
			And.HaveCount(2, "Left black knight should have 2 possible moves after the undo");
		}
	}
}
