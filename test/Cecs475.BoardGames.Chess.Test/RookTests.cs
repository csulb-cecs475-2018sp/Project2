using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class RookTests : ChessTest {
		[Fact]
		public void WhiteCastle() {
			ChessBoard board = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"f1, b5",
				"c7, c6",
				"g1, f3",
				"c6, b5",
				"e1, g1"
			);
			board.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Rook, "White's rook at position f1 due to castle");
			board.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.King, "White's king at position g1 due to castle");
			board.GetPieceAtPosition(Pos("f1")).Player.Should().Be(1, "Rook in castle owned by white");
			board.GetPieceAtPosition(Pos("g1")).Player.Should().Be(1, "King in castle owned by white");
		}

		[Fact]
		public void NoCastlingIfRookMoved() {
			ChessBoard b = CreateBoardFromPositions(
			Pos("a8"), ChessPieceType.Rook, 2,//black
			Pos("e1"), ChessPieceType.King, 1,//white
			Pos("e8"), ChessPieceType.King, 2//black
			);
			Apply(b, "e1, e2");
			Apply(b, "a8, a7");
			Apply(b, "e2, e1");
			Apply(b, "a7, a8");
			Apply(b, "e1, e2");
			var movesPossible = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(movesPossible, Pos("e8"));
			//the syntax of the next two lines is incorrect
			kingMoves.Should().HaveCount(5, "king at e8 cannot castle after the rook has already moved, even if the rook moves back.")
				 .And.BeEquivalentTo(Move("e8, e7"), Move("e8, d7"), Move("e8, d8"), Move("e8, f7"), Move("e8, f8"));

		}

		/// <summary>
		/// Rook only has 2 moves
		/// </summary>
		[Fact]
		public void Rook_PossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				"h2, h4",
				"h7, h5",
				"h1, h3", // move rook out
				"b7, b6",
				"g2, g3", // move pawn up one blocking rooks previously possible moves to the left
				"b6, b5"
			);

			var possMoves = b.GetPossibleMoves();
			var twoMoves = GetMovesAtPosition(possMoves, Pos("h3"));
			twoMoves.Should().HaveCount(2, "the rook should only be able to move 1 or 2 spaces down")
				.And.BeEquivalentTo(Move("h3, h2"), Move("h3, h1"));
		}

		/// <summary>
		/// Tests a comparison between UndoLastMove and performing a move back to starting
		/// position for player 1's ability to perform castling. This test moves the kingside
		/// rook and then undoes that move. It then checks to see if castling is a possible move
		/// (it should be). Then the test moves the rook forward and back and tests to see if
		/// castling is still a possible move (it shouldn't be).
		/// </summary>
		[Fact]
		public void CastlingAfterUndoVersusMoveBack() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.Rook, 1,
				 Pos("e8"), ChessPieceType.King, 2
			);

			Apply(b, Move("h1, h3"));
			b.UndoLastMove();
			var possMoves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			kingMoves.Should().HaveCount(6, "king should be able to castle").And.BeEquivalentTo(Move("e1, f1"),
				 Move("e1, d1"), Move("e1, e2"), Move("e1, d2"), Move("e1, f2"), Move("e1, g1"));
			Apply(b, Move("h1, h3"), Move("e8, e7"), Move("h3, h1"), Move("e7, e8"));
			possMoves = b.GetPossibleMoves();
			kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			kingMoves.Should().HaveCount(5, "castling should not be allowed after the rook has moved from starting position").And.BeEquivalentTo(Move("e1, f1"),
				 Move("e1, d1"), Move("e1, e2"), Move("e1, d2"), Move("e1, f2"));
		}

		/// <summary>
		/// Puts the black rook in a restricted position and tests to see if the possible moves
		/// from this position are correct.
		/// </summary>
		[Fact]
		public void RookPossibleMoves() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("c5"), ChessPieceType.Rook, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("c4"), ChessPieceType.Pawn, 1,
				 Pos("b5"), ChessPieceType.Pawn, 1,
				 Pos("d5"), ChessPieceType.Pawn, 2,
				 Pos("e4"), ChessPieceType.Pawn, 1,
				 Pos("c7"), ChessPieceType.Queen, 1
			 );

			Apply(b, Move("e4, e5"));
			var possMoves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(possMoves, Pos("c5"));
			kingMoves.Should().HaveCount(4, "rook can attack one of the enemy pawns, the queen, or move up one space").And.BeEquivalentTo(Move("c5, c6"),
				 Move("c5, b5"), Move("c5, c4"), Move("c5, c7"));
		}

		[Fact]
		public void TwoRooksKingInCheck() {
			//short castling
			//White MoVe Next
			//Checking White King
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e4"), ChessPieceType.Rook, 2,
				 Pos("f3"), ChessPieceType.Rook, 2

			);
			var possible = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(possible, Pos("e1"));
			b.IsCheck.Should().BeTrue("White's king is in check from pawn at a7");
			expected.Count().Should().Be(2, "There should be " +
				 "2 possible moves to cancel the checking of White's king");
			Apply(b, Move("e1, d2"));
			Apply(b, Move("f3, f2"));
			possible = b.GetPossibleMoves();
			expected = GetMovesAtPosition(possible, Pos("d2"));
			b.IsCheck.Should().BeTrue("White's king is in check from pawn at a7");
			expected.Count().Should().Be(4, "There should be " +
				 "4 possible moves to cancel the checking of White's king");
		}

		[Fact]
		public void StressTestRooks() {
			ChessBoard b = CreateBoardFromMoves(
				 "h2, h4",
				 "h7, h5"
			);
			//White Move Next
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("h1"));
			expected.Should().HaveCount(2, "A White Rook at this location has 2 Possible Moves");
			expected = GetMovesAtPosition(poss, Pos("h8"));
			expected.Should().HaveCount(0, "White Player's turn Black has 0 possible moves");
			Apply(b, Move("h1,h3"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("h8"));
			expected.Should().HaveCount(2, "A Black Rook at this location has 2 Possible Moves");
			Apply(b, Move("h8,h6"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("h3"));
			expected.Should().HaveCount(9, "A White Rook at this location has 9 Possible Moves");
			Apply(b, Move("h3,d3"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("h6"));
			expected.Should().HaveCount(9, "A Black Rook at this location has 9 Possible Moves");
			Apply(b, Move("h6,f6"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("d3"));
			expected.Should().HaveCount(11, "A White Rook at this location has 11 Possible Moves");
			Apply(b, Move("d3,d7"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("f6"));
			expected.Should().HaveCount(11, "A Black Rook at this location has 11 Possible Moves");
			Apply(b, Move("f6,f2"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("d7"));
			expected.Should().HaveCount(7, "A White Rook at this location has 7 Possible Moves");
			Apply(b, Move("d7,e7"));
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("f2"));
			expected.Should().HaveCount(0, "Black King is in check, Black Rook has 0 Possible Move");
		}

		/// <summary>
		/// Check the possible of rooks in all directions. 
		/// Check if rook can't go past black pieces.
		/// </summary>
		[Fact]
		public void PossibleRookMoves() {
			//Check White Rook's possible moves I
			ChessBoard b = new ChessBoard();
			var possible = b.GetPossibleMoves();
			var rookMoves = GetMovesAtPosition(possible, Pos("a1"));
			rookMoves.Count().Should().Be(0, "There should not be any possible moves for white rook");

			//Check White Rook's possible moves II
			Apply(b, "a2, a4");
			Apply(b, "h7, h6");
			Apply(b, "a1, a3");
			Apply(b, "e7, e6");
			possible = b.GetPossibleMoves();
			rookMoves = GetMovesAtPosition(possible, Pos("a3"));
			rookMoves.Count().Should().Be(9, "white rook can go down and to the right");

			//Check White Rook's possible moves III
			Apply(b, "a3, e3");
			Apply(b, "f8, a3");
			possible = b.GetPossibleMoves();
			rookMoves = GetMovesAtPosition(possible, Pos("e3"));
			rookMoves.Count().Should().Be(10, "white rook can go up to 3 for capture, left up to 4, right up to 3");
		}
	}
}
