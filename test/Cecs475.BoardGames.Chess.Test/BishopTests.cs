using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class BishopTests : ChessTest {
		/// <summary>
		/// Test possible moves for two Bishops belonging to White Player
		/// </summary>
		[Fact]
		public void WhiteBishopTest() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.King, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 1), new ChessPiece(ChessPieceType.Bishop, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 6), new ChessPiece(ChessPieceType.Bishop, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 5), new ChessPiece(ChessPieceType.Queen, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 0), new ChessPiece(ChessPieceType.Rook, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 2), new ChessPiece(ChessPieceType.Knight, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 7), new ChessPiece(ChessPieceType.Bishop, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 0), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 2), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 5), new ChessPiece(ChessPieceType.Pawn, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 7), new ChessPiece(ChessPieceType.Pawn, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			var possMoves = b.GetPossibleMoves();

			var bishopOneMoves = GetMovesAtPosition(possMoves, Pos("b5"));
			var bishopTwoMoves = GetMovesAtPosition(possMoves, Pos("g5"));

			bishopOneMoves.Should().HaveCount(4, "Bishop at B5 (White Space) has Four Possible Capture Moves in Total")
				 .And.Contain(Move("b5, a6"), "Bishop (b5) takes Rook (a6)")
				 .And.Contain(Move("b5, c6"), "Bishop (b5) takes Knight (a6)")
				 .And.Contain(Move("b5, a4"), "Bishop (b5) takes Pawn (a4)")
				 .And.Contain(Move("b5, c4"), "Bishop (b5) takes Pawn (c4)");

			bishopTwoMoves.Should().HaveCount(4, "Bishop at G5 has Four Possible Capture Moves in Total")
				 .And.Contain(Move("g5, f6"), "Bishop (g5) takes Queen (f6)")
				 .And.Contain(Move("g5, h6"), "Bishop (g5) takes Bishop (h6)")
				 .And.Contain(Move("g5, f4"), "Bishop (g5) takes Pawn (f4)")
				 .And.Contain(Move("g5, h4"), "Bishop (g5) takes Pawn (h4)");
		}

		[Fact]

		public void WhiteUndoMove() {
			ChessBoard board = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"d2, d4",
				"d7, d6",
				"c2, c3",
				"h7, h5",
				"d4, e5",
				"c8, f5",
				"g2, g3",
				"f5, e4",
				"f1, g2",
				"e4, g2",
				"g1, h3",
				"g2, h1"
			);

			board.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Bishop, "Black's bishop took rook on h1");
			board.GetPieceAtPosition(Pos("h1")).Player.Should().Be(2, "Black's bishop is at position h1");
			board.CurrentAdvantage.Should().Be(Advantage(2, 8), "Black has a bishop and rook advantage");

			board.UndoLastMove();
			board.UndoLastMove();
			board.UndoLastMove();

			board.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "White's rook has not been moved or captured");
			board.GetPieceAtPosition(Pos("g2")).PieceType.Should().Be(ChessPieceType.Bishop, "White's bishop has not been captured at g2");
			board.GetPieceAtPosition(Pos("g2")).Player.Should().Be(1, "The bishop on g2 should belong to white");
			board.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Bishop, "The piece at e4 should be a black bishop");
			board.GetPieceAtPosition(Pos("e4")).Player.Should().Be(2, "The bishop at e4 should belong to black");
			board.CurrentAdvantage.Should().Be(Advantage(0, 0), "There should currently be no advantage on the board");
		}

		/// <summary>
		/// Testing bishop possible moves
		/// </summary>
		[Fact]
		public void BishopPossibleMoves2() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"d7, d5",
				"b2, b4"
			);
			var possMoves = b.GetPossibleMoves();
			var bishopMoves = GetMovesAtPosition(possMoves, Pos("c8"));
			bishopMoves.Should().HaveCount(5, "kight can move in one diagonal from b8")
				.And.Contain(Move("c8, d7"))
				.And.Contain(Move("c8, e6"))
				.And.Contain(Move("c8, f5"))
				.And.Contain(Move("c8, g4"))
				.And.Contain(Move("c8, h3"));
		}

		/// <summary>
		/// Check a lot of different possible moves for bishop (player 1 and 2)
		/// </summary>
		[Fact]
		public void checkBishopPossibleMoves() {
			ChessBoard b = new ChessBoard();

			var bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c1"));
			bishopMoves.Should().HaveCount(0, "there is no possible move at start for p1's bishop");
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f1"));
			bishopMoves.Should().HaveCount(0, "there is no possible move at start for p1's bishop");

			Apply(b, "b2, b3"); // one space pawn's move from p1
			Apply(b, "e7, e5"); // two space pawn's move from p2
			Apply(b, "d2, d3"); // one space pawn's move from p1
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f8"));
			bishopMoves.Should().HaveCount(5, "there is only 5 possible moves for bishop at this stage of game")
				.And.OnlyContain(m => m.MoveType == ChessMoveType.Normal);

			Apply(b, "f8, d6"); // two space bishop's move from p2
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("d6"));
			bishopMoves.Should().HaveCount(0, "there is only 0 possible moves for p2's bishop at this stage of game, because it's not p2's turn.");

			Apply(b, "c1, a3"); // two space bishop's move from p1
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("d6"));
			bishopMoves.Should().HaveCount(5, "there is only 5 possible moves for p2's bishop at this stage of game.");
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c3"));
			bishopMoves.Should().HaveCount(0, "there is only 0 possible moves for p1's bishop at this stage of game, because it's not p1's turn.");

			Apply(b, "d6, a3"); // Take p1's bishop with p2's bishop
			Apply(b, "g2, g3"); // one space move from p1's pawn
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("a3"));
			bishopMoves.Should().HaveCount(7, "there is only 07possible moves for p2's bishop at this stage of game.");

			Apply(b, "a3, c1"); // move p2's bishop
			Apply(b, "f1, h3"); // move p1's bishop
			Apply(b, "h7, h6"); // move p2's pawn
			Apply(b, "h3, f5"); // move p1's bishop
			Apply(b, "d7, d6"); // move p2's pawn
			Apply(b, "d3, d4"); // move p1's pawn
			Apply(b, "g7, g6"); // move p2's pawn
			bishopMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("f5"));
			bishopMoves.Should().HaveCount(8, "there is only 8 possible moves for p1's bishop at this stage of game.");
		}

		///<summary>
		/// Attempts a white bishop to capture black queen
		///</summary>
		[Fact]
		public void BlackQueenCapture() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 4), ChessPieceType.Queen, 2, //black queen to be captured
				 Pos(2, 3), ChessPieceType.Bishop, 1,
				 Pos(2, 6), ChessPieceType.King, 2,
				 Pos(5, 6), ChessPieceType.Bishop, 2, //white bishop to capture
				 Pos(6, 1), ChessPieceType.Rook, 1,
				 Pos(6, 4), ChessPieceType.King, 1);

			//check if queen can be captured
			var poss = cb.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos(2, 3));
			expected.Should().Contain(Move("d6, e7")).And.HaveCount(2, "Bishop can take Queen or not take Queen");

			//capture queen
			Apply(cb, Move("d6, e7"));
			cb.GetPieceAtPosition(Pos("e7")).Player.Should().Be(1, "P1 Bishop captured P2 Queen diagonally");
		}

		///<summary>
		/// Checks possibles moves for black bishop
		///</summary>
		[Fact]
		public void MovesForBlackBishop() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 1), ChessPieceType.King, 2,
				 Pos(1, 5), ChessPieceType.Rook, 2,
				 Pos(2, 4), ChessPieceType.Bishop, 2, //black bishop to move
				 Pos(4, 2), ChessPieceType.Rook, 1,
				 Pos(3, 5), ChessPieceType.Pawn, 1,
				 Pos(6, 5), ChessPieceType.King, 1);

			//check all positions for which a black bishop can move
			var moves = cb.GetPossibleMoves();
			var expected = GetMovesAtPosition(moves, Pos(2, 4));
			expected.Should().BeEmpty("Black Bishop cannot move");
			expected.Should().NotContain(Move("e6, c4")).And.HaveCount(0, "Black Bishop cannot take White Rook");
		}

		[Fact]
		public void BishopPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
					  "d2, d3",
					  "g8, f6", // black knight 
					  "b2, b4",
					  "c7, c5",
					  "c1, b2",
					  "e7, e6",
					  "b2, e5", // white bishop
					  "g7, g5");

			var possMoves = b.GetPossibleMoves();
			var whiteBishop = GetMovesAtPosition(possMoves, Pos("e5"));
			whiteBishop.Should().HaveCount(9, "White Bishop should have 9 moves, two of them should capture one of the black's knights")
			.And.Contain(Move("e5,d4")).And.Contain(Move("e5,c3")).And.Contain(Move("e5,b2"))
			.And.Contain(Move("e5,f6")).And.Contain(Move("e5,g3")).And.Contain(Move("e5,f6"))
			.And.Contain(Move("e5,d6")).And.Contain(Move("e5,c7")).And.Contain(Move("e5,b8"));
		}
	}
}
