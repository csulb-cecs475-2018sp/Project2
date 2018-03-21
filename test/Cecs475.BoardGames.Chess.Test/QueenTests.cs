using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class QueenBoardTests : ChessTest {
		/// <summary>
		/// White's Queen attacks Black's Queen and tests advantage, then the
		/// advantage of the game should reset to previous state after undoing
		/// the attack
		/// </summary>
		[Fact]
		public void WhiteUndoQueenAttack() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.King, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 3), new ChessPiece(ChessPieceType.Queen, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 3), new ChessPiece(ChessPieceType.Queen, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			var possMoves = b.GetPossibleMoves();

			var queenMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			queenMoves.Should().Contain(Move("d1, d5"), "White Queen should be able to Black Queen");

			Apply(b, Move("d1, d5"));

			b.CurrentAdvantage.Should().Be(Advantage(1, 9), "Advantage shifts towards White for taking Black's Queen");

			b.UndoLastMove();

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Advantage resets to 0, 0 after undoing move");
		}

		//Checking possible moves for queen
		[Fact]
		public void PossibleMovesQueen() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "e7, e5",
				 "f1, c4",
				 "b8, c6");
			var possMoves = b.GetPossibleMoves();
			var expectedQueenMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			expectedQueenMoves.Should().HaveCount(4, "Queen can only move diagonally due to other pieces blocking it")
				 .And.Contain(Move("d1, e2"))
				 .And.Contain(Move("d1, f3"))
				 .And.Contain(Move("d1, g4"))
				 .And.Contain(Move("d1, h5"));
		}

		/// <summary>
		/// Test a simple capture undo
		/// </summary>
		[Fact]
		public void TestCaptureUndo() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e4"), ChessPieceType.Queen, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("h8"), ChessPieceType.King, 2,
				 Pos("b7"), ChessPieceType.Pawn, 2
			);

			b.CurrentAdvantage.Should().Be(Advantage(1, 8), "White has a current advantage of 8");
			b.GetPieceAtPosition(Pos("e4")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 1), "Position e4 should be a Queen for player 1");
			b.GetPieceAtPosition(Pos("b7")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 2), "Position b7 should be a Pawn for player 2");

			Apply(b, Move("e4, b7"));

			b.CurrentAdvantage.Should().Be(Advantage(1, 9), "Black has a current advantage of 9 now");
			b.GetPieceAtPosition(Pos("b7")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 1), "Position b7 should be a Queen for player 1 after move (e4, b7)");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("e4")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 1), "Position e4 should be a Queen for player 1 after undo");
			b.GetPieceAtPosition(Pos("b7")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 2), "Position b7 should be a Pawn for player 2 undo");

		}

		/// <summary>
		/// The test checks possible moves for queen when the friendly king is in check or not. 
		/// Test : - GetPossibleMoves
		/// Player: - Black
		/// Result: - In the given layout of the board, black queen should be able to capture all white pieces 
		/// in the board. However, when the black king is at check, the queen's possible move is restricted. In
		/// addition, queen cannot capture two enemy pieces in one move. 
		/// </summary>
		[Fact]
		public void QueenPossibleMoves() {
			ChessBoard board = CreateBoardFromPositions(
				 Pos("a2"), ChessPieceType.Rook, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e4"), ChessPieceType.Pawn, 1,
				 Pos("f5"), ChessPieceType.Pawn, 1,
				 Pos("d5"), ChessPieceType.Queen, 2,
				 Pos("b8"), ChessPieceType.King, 2);

			//apply move to a white piece
			Apply(board, "e1,e2");
			var possibleMoves = board.GetPossibleMoves();
			var queenMoves = GetMovesAtPosition(possibleMoves, Pos("d5"));

			queenMoves.Should().Contain(Move("d5,e4"), "The queen should be able to capture white pawn at e4")
				 .And.Contain(Move("d5,f5"), "The queen should be able to capture white pawn at f5")
				 .And.Contain(Move("d5,a2"), "The queen should be able to capture the white rook at a2");

			board.UndoLastMove();

			//apply move to white rook so black king is in check
			Apply(board, "a2,b2");

			possibleMoves = board.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possibleMoves, Pos("d5"));
			queenMoves.Should().NotContain(Move("d5,e4"), "The queen should not be able to capture because of check to king")
				 .And.NotContain(Move("d5,f5"), "The queen should not be able to capture because of check to king");

			board.UndoLastMove();

			//apply move to a white pawn
			Apply(board, "e4,e5");

			possibleMoves = board.GetPossibleMoves();
			queenMoves = GetMovesAtPosition(possibleMoves, Pos("d5"));
			queenMoves.Should().Contain(Move("d5,e5"), "The queen can capture the adjacent white pawn at e5")
				 .And.NotContain(Move("d5,f5"), "The queen cannot capture two pawns at the same ");

		}

		/// <summary>
		/// Testing undo of queen capture
		/// </summary>
		[Fact]
		public void UndoQueenCapture() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"e7, e5",
				"c4, c5",
				"e5, e4",
				"d1, c2",
				"d7, d5"
			);

			var possMoves = b.GetPossibleMoves();
			var queenCaptureExpected = GetMovesAtPosition(possMoves, Pos("c2"));
			b.GetPieceAtPosition(Pos("c2")).PieceType.ShouldBeEquivalentTo(ChessPieceType.Queen, "the queen was moved from d1 -> c2");

			queenCaptureExpected.Should().HaveCount(7, "queen can move diagonally in three different ways and vertically in one from c2")
				.And.Contain(Move("c2, b3"))
				.And.Contain(Move("c2, a4"))
				.And.Contain(Move("c2, d3"))
				.And.Contain(Move("c2, c3"))
				.And.Contain(Move("c2, c4"))
				.And.Contain(Move("c2, d1"))
				.And.Contain(Move(Pos("c2"), Pos("e4"), ChessMoveType.Normal));

			// Apply the queen capture
			Apply(b, Move(Pos("c2"), Pos("e4"), ChessMoveType.Normal));
			var queen = b.GetPieceAtPosition(Pos("e4"));
			queen.Player.Should().Be(1, "queen performed capture");
			queen.PieceType.Should().Be(ChessPieceType.Queen, "queen performed capture");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "player 1 captured player 2's pawn with a queen");

			// Undo the move and check the board state
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "queen capture was undone");
			queen = b.GetPieceAtPosition(Pos("c2"));
			queen.Player.Should().Be(1, "queen capture was undone");
			queen.PieceType.Should().Be(ChessPieceType.Queen, "queen capture was undone");
			var captured = b.GetPieceAtPosition(Pos("e4"));
			captured.Player.Should().Be(2, "queen capture was undone");
			var boardIntegrity = b.GetPieceAtPosition(Pos("c5"));
			boardIntegrity.Player.Should().Be(1, "player 1 has a piece at e4");
			boardIntegrity.PieceType.Should().Be(ChessPieceType.Pawn, "queen capture was undone");
		}

		//1st undo move checking Black Queen
		[Fact]
		public void undoBlackTest() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "h7, h5",
				 "b1, c3",
				 "d7, d5"
			);
			//White Move Next
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("c3"));
			expected.Should().Contain(Move("c3, b1"))
				 .And.Contain(Move("c3, a2"))
				 .And.Contain(Move("c3, b5"))
				 .And.Contain(Move("c3, d5"))//haha
				 .And.Contain(Move("c3, e4"))
				 .And.HaveCount(5, "A White Knight has 5 possible move at this position" +
				 "And one of them should be able to capture Black's Pawn");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0),
				 "no operations have changed the advantage");
			Apply(b, Move("c3, d5")); /// White Knight get the Pawn
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "White's Knight captures Black's Pawn");
			Apply(b, Move("c7, c6"));
			Apply(b, Move("d5, e7"));
			b.CurrentAdvantage.Should().Be(Advantage(1, 2), "White's Kinght captures Black's Pawn");
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("d8"));
			expected.Should().Contain(Move("d8, e7")) // Black Queen Captures White's Knight
				.And.Contain(Move("d8, d2"))  // Black Queen Captures White's Pawns.
				.And.HaveCount(10, "The Black Queen has 10 possible move at this position" +
				"Black Queen Should be able to captures the White's Knight and Pawns");
			b.UndoLastMove();
			b.UndoLastMove();
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("d8"));
			//after the Undo, the Black Queen Only have 3 moves
			expected.Should().Contain(Move("d8, d5")) // Black Queen Captures White Knight
				.And.HaveCount(3, "The Black QUeen has 3 Possible moves" +
				"And one of them should be able to capture Black's Pawn");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Undo 1 Move White Player only captures 1 pawn");
		}

		[Fact]
		public void undoWhiteTest() {
			ChessBoard b = CreateBoardFromMoves(
				 "h2, h3",
				 "h7, h6",
				 "h3, h4",
				 "h6, h5"
			);
			//White Move Next
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("h4"));
			expected.Should().HaveCount(0, "A White Pawn at this location has no Possible Move");
			expected = GetMovesAtPosition(poss, Pos("h1"));
			expected.Should().HaveCount(2, "A White Rook at this location has 2 Possible Moves");
			expected = GetMovesAtPosition(poss, Pos("g1"));
			expected.Should().HaveCount(2, "A White Knight at this location has 2 Possible Moves");
			b.UndoLastMove();
			b.UndoLastMove();
			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("h3"));
			expected.Should().HaveCount(1, "A White Pawn at this location has 1 Possible Move");
			expected = GetMovesAtPosition(poss, Pos("h1"));
			expected.Should().HaveCount(1, "A White Rook at this location has 1 Possible Move");
			expected = GetMovesAtPosition(poss, Pos("g1"));
			expected.Should().HaveCount(1, "A White Knight at this location has 1 Possible Moves");
		}

		/// <summary>
		/// The purpose of this test is to test that the queen (who has a lot of moves usually)
		/// gets only a few moves when his king is in check
		/// </summary>
		[Fact]
		public void BlackQueenPossibleMovesOnCheck() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("d1"), ChessPieceType.King, 1,
				 Pos("c1"), ChessPieceType.Queen, 1,
				 Pos("e3"), ChessPieceType.Rook, 1,
				 Pos("c2"), ChessPieceType.Pawn, 1,
				 Pos("h8"), ChessPieceType.King, 2,
				 Pos("c7"), ChessPieceType.Queen, 2
			);
			var possMoves = b.GetPossibleMoves();
			var whiteQueenBlocked = GetMovesAtPosition(possMoves, Pos("c1"));
			whiteQueenBlocked.Should().HaveCount(5, "Queen directions are a bit blocked by freindly pieces")
				 .And.Contain(Move("c1, d2"))
				 .And.Contain(Move("c1, b2"))
				 .And.Contain(Move("c1, a3"))
				 .And.Contain(Move("c1, a1"))
				 .And.Contain(Move("c1, b1"));
			Apply(b, "c1, a1");
			possMoves = b.GetPossibleMoves();
			var BlackQueenMoves = GetMovesAtPosition(possMoves, Pos("c7"));
			BlackQueenMoves.Should().HaveCount(3, "Queen should protect king from check by queen in a1")
				 .And.Contain(Move("c7, c3"))
				 .And.Contain(Move("c7, e5"))
				 .And.Contain(Move("c7, g7"));
			Apply(b,
				 "h8, h7",
				 "c2, c3",
				 "c7, c6",
				 "a1, b1");
			possMoves = b.GetPossibleMoves();
			BlackQueenMoves = GetMovesAtPosition(possMoves, Pos("c6"));
			BlackQueenMoves.Should().HaveCount(2, "Queen should protect king from check by queen in b1")
				 .And.Contain(Move("c6, e4"))
				 .And.Contain(Move("c6, g6"));
		}

		/* 
		2. On the black side, 3 pawns are moved:
		c7 to c6, d7 to d5, and e7 to e6
		Black queen is tested to see if there are 9 possible moves
		*/
		[Fact]
		public void CountBlackQueenMovesWith3PawnsOutOfWay() {
			//start with moving black pawns in front of queen
			ChessBoard board = CreateBoardFromMoves(
				"e2,e3",
				"c7, c6",
				"a2,a3",
				"d7, d5",
				"b2,b3",
				"e7, e6",
				"d2,d3");
			var possMoves = board.GetPossibleMoves();
			var blackQueenNoMovesExpected = GetMovesAtPosition(possMoves, Pos("d8")); //initial moves for black queen
			blackQueenNoMovesExpected.Should().HaveCount(9,
			"the black queen should have 9 moves with pawns in c6, d5, and e6").And.Contain(Move("d8, h4"));
		}
	}
}
