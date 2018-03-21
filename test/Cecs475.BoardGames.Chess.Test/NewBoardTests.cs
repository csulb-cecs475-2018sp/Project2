using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class NewBoardTests : ChessTest {
		/// <summary>
		/// Simple facts about "new" boards.
		/// </summary>
		[Fact]
		public void NewChessBoard() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos(7, 0)).Player.Should().Be(1, "Player 1 should be in lower left of board");
			b.GetPieceAtPosition(Pos(0, 0)).Player.Should().Be(2, "Player 2 should be in upper left of board");
			b.GetPieceAtPosition(Pos(4, 0)).Player.Should().Be(0, "Middle left of board should be empty");
			// Test a few select piece locations.
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "White's king at position (7,4)");
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black's king at position (0,4)");
			// Test other properties
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");
		}

		/// <summary>
		/// Tests all possible opening moves from game start
		/// </summary>
		[Fact]
		public void OpeningMoves() {
			ChessBoard b = new ChessBoard();

			var possMoves = b.GetPossibleMoves();

			possMoves.Should().HaveCount(20, "P1 Should only have 20 Possible Moves from start of Game: (8 Pawns * 2 Moves each) + (2 Knights * 2 Moves Each)");

			var knightMoves = GetMovesAtPosition(possMoves, Pos("b1"));

			knightMoves.Should().HaveCount(2, "Knight at B1 only has two moves from start of game")
				 .And.Contain(Move("b1, a3"), "Starting position to vertical L left")
				 .And.Contain(Move("b1, c3"), "Starting position vertical L right");

			knightMoves = GetMovesAtPosition(possMoves, Pos("g1"));

			knightMoves.Should().HaveCount(2, "Knight at B1 only has two moves from start of game")
				 .And.Contain(Move("g1, f3"), "Starting position to vertical L left")
				 .And.Contain(Move("g1, h3"), "Starting position vertical L right");

			// Switch from Player One to Player Two by playing dummy move
			Apply(b, Move("b2, b4"));

			possMoves = b.GetPossibleMoves();

			knightMoves = GetMovesAtPosition(possMoves, Pos("b8"));

			knightMoves.Should().HaveCount(2, "Knight at B1 only has two moves from start of game")
				 .And.Contain(Move("b8, a6"), "Starting position to vertical L left")
				 .And.Contain(Move("b8, c6"), "Starting position vertical L right");

			knightMoves = GetMovesAtPosition(possMoves, Pos("g8"));

			knightMoves.Should().HaveCount(2, "Knight at B1 only has two moves from start of game")
				 .And.Contain(Move("g8, f6"), "Starting position to vertical L left")
				 .And.Contain(Move("g8, h6"), "Starting position vertical L right");

			possMoves.Should().HaveCount(20, "P2 Should only have 20 Possible Moves from start of Game: (8 Pawns * 2 Moves each) + (2 Knights * 2 Moves Each)");
		}

		//Checking moves for black pieces
		[Fact]
		public void NotYourTurn() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves();
			var expectedMoves = GetMovesAtPosition(possMoves, Pos("a7"));
			expectedMoves.Should().HaveCount(0, "It is not black's turn");

		}

		/// <summary>
		/// Test Initial state of the board
		/// </summary>

		[Fact]
		public void TestInitialState() {
			ChessBoard b = new ChessBoard();

			b.GetPieceAtPosition(Pos("d8")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 2), "Position d8 should be a Queen for player 2");
			b.GetPieceAtPosition(Pos("d1")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 1), "Position d1 should be a Queen for player 1");

			b.GetPieceAtPosition(Pos("a8")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Rook, 2), "Position a8 should be a Rook for player 2");
			b.GetPieceAtPosition(Pos("h8")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Rook, 2), "Position h8 should be a Rook for player 2");
			b.GetPieceAtPosition(Pos("a1")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Rook, 1), "Position a1 should be a Rook for player 1");
			b.GetPieceAtPosition(Pos("h1")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Rook, 1), "Position h1 should be a Rook for player 1");

			b.PositionIsEmpty(Pos("d4")).Should().BeTrue("Position d4 should be empty");
		}

		[Fact] //b
		public void RookStartingPosition() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "Initial position of Rook should be at a8");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "Initial position of Rook should be at h8");

		}

		/// <summary>
		/// Check that the initial state of the game is correct.
		/// </summary>
		[Fact]
		public void InitialBoardState() {
			ChessBoard b = new ChessBoard();

			// Check row 1
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("a1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "piece is a Knight");
			b.GetPieceAtPosition(Pos("b1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "piece is a Bishop");
			b.GetPieceAtPosition(Pos("c1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "piece is a Queen");
			b.GetPieceAtPosition(Pos("d1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("e1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Bishop, "piece is a Bishop");
			b.GetPieceAtPosition(Pos("f1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "piece is a Knight");
			b.GetPieceAtPosition(Pos("g1")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("h1")).Player.Should().Be(1, "piece is white");

			// Check row 2
			b.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("a2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("b2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("b2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("c2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("c2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("d2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("d2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("e2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("e2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("f2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("f2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("g2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("g2")).Player.Should().Be(1, "piece is white");

			b.GetPieceAtPosition(Pos("h2")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("h2")).Player.Should().Be(1, "piece is white");

			// Check row 3
			b.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("b3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("d3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("e3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("f3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("g3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("h3")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			// Check row 4
			b.GetPieceAtPosition(Pos("a4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("b4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("d4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("f4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("g4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("h4")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			// Check row 5
			b.GetPieceAtPosition(Pos("a5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("b5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("c5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("f5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("g5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("h5")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			// Check row 6
			b.GetPieceAtPosition(Pos("a6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("b6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("c6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("f6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("g6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.GetPieceAtPosition(Pos("h6")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			// Check row 7
			b.GetPieceAtPosition(Pos("a7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("a7")).Player.Should().Be(2);

			b.GetPieceAtPosition(Pos("b7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("b7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("c7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("c7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("d7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("d7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("e7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("e7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("f7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("f7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("g7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("g7")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("h7")).PieceType.Should().Be(ChessPieceType.Pawn, "piece is a Pawn");
			b.GetPieceAtPosition(Pos("h7")).Player.Should().Be(2, "piece is black");

			// Check row 8
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("a8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "piece is a Knight");
			b.GetPieceAtPosition(Pos("b8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("c8")).PieceType.Should().Be(ChessPieceType.Bishop, "piece is a Bishop");
			b.GetPieceAtPosition(Pos("c8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "piece is a Queen");
			b.GetPieceAtPosition(Pos("d8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("e8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Bishop, "piece is a Bishop");
			b.GetPieceAtPosition(Pos("f8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight, "piece is a Knight");
			b.GetPieceAtPosition(Pos("g8")).Player.Should().Be(2, "piece is black");

			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("h8")).Player.Should().Be(2, "piece is black");

			// Player 1 moves first
			b.CurrentPlayer.Should().Be(1, "Player 1 moves first");

			// Score should be 0 - 0
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "score should be tied at 0");
		}

		[Fact]
		public void Player1QueenStartingMoves() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves();
			var blockedQueen = GetMovesAtPosition(possMoves, Pos("d1"));
			blockedQueen.Should().BeEmpty("Player1 queen should have no moves at start");
		}

		/// <summary>
		/// Try undo move before any move is created.
		/// </summary>
		[Fact]
		public void IllegalUndo() {
			ChessBoard b = new ChessBoard();
			Exception ex = null;

			try {
				b.UndoLastMove();
			}
			catch (Exception exception) {
				ex = exception;
			}

			ex.Should().BeOfType<InvalidOperationException>("There are no moves made from new board");

		}

		/// <summary>
		/// Testing the initial starting board state
		/// </summary>
		[Fact]
		public void InitialBoardState2() {
			ChessBoard b = new ChessBoard();

			var currentPlayer = b.CurrentPlayer;
			currentPlayer.Should().Be(1, "Player 1 should be the first to make a move");
			var stalemate = b.IsStalemate;
			stalemate.Should().BeFalse("there should not be a stalement at the beginning of the game");
			var checkmate = b.IsCheckmate;
			checkmate.Should().BeFalse("there should not be a checkmate at the beginning of the game");
			var check = b.IsCheck;
			check.Should().BeFalse("there should not be a check at the beginning of the game");
			var drawCounter = b.DrawCounter;
			drawCounter.Should().Be(0, "draws should not have happened at the beginning of the game");
			var moveHistory = b.MoveHistory;
			moveHistory.Should().BeEmpty("no moves should have been made yet");
			var isFinished = b.IsFinished;
			isFinished.Should().BeFalse("the game should not be finished at the beginning");
			var currentAdvantage = b.CurrentAdvantage;
			currentAdvantage.Player.Should().Be(0, "no player should have an advantage at the beginning of the game");
			currentAdvantage.Advantage.Should().Be(0, "no player should have an advantage at the beginning of the game");
			char[] columns = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
			foreach (var column in columns) {
				var pawnWhite = b.GetPieceAtPosition(Pos(string.Format("{0}2", column)));
				var pawnBlack = b.GetPieceAtPosition(Pos(string.Format("{0}7", column)));
				pawnWhite.PieceType.Should().Be(ChessPieceType.Pawn, "pieces in row 2 should be pawns at the beginning of the game");
				pawnBlack.PieceType.Should().Be(ChessPieceType.Pawn, "pieces in row 7 should be pawns at the beginning of the game");
				if (column == 'a') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Rook, string.Format("piece at ({0},1) should be a rook at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Rook, string.Format("piece at ({0},8) should be a rook at the beginning of the game", column));
				}
				else if (column == 'b') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Knight, string.Format("piece at ({0},1) should be a knight at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Knight, string.Format("piece at ({0},8) should be a knight at the beginning of the game", column));
				}
				else if (column == 'c') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Bishop, string.Format("piece at ({0},1) should be a bishop at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Bishop, string.Format("piece at ({0},8) should be a bishop at the beginning of the game", column));
				}
				else if (column == 'd') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Queen, string.Format("piece at ({0},1) should be a queen at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Queen, string.Format("piece at ({0},8) should be a queen at the beginning of the game", column));
				}
				else if (column == 'e') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.King, string.Format("piece at ({0},1) should be a King at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.King, string.Format("piece at ({0},8) should be a King at the beginning of the game", column));
				}
				else if (column == 'f') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Bishop, string.Format("piece at ({0},1) should be a Bishop at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Bishop, string.Format("piece at ({0},8) should be a Bishop at the beginning of the game", column));
				}
				else if (column == 'g') {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Knight, string.Format("piece at ({0},1) should be a knight at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Knight, string.Format("piece at ({0},8) should be a knight at the beginning of the game", column));
				}
				else {
					var pieceWhite = b.GetPieceAtPosition(Pos(string.Format("{0}1", column)));
					var pieceBlack = b.GetPieceAtPosition(Pos(string.Format("{0}8", column)));
					pieceWhite.PieceType.Should().Be(ChessPieceType.Rook, string.Format("piece at ({0},1) should be a rook at the beginning of the game", column));
					pieceBlack.PieceType.Should().Be(ChessPieceType.Rook, string.Format("piece at ({0},8) should be a rook at the beginning of the game", column));
				}
			}
		}

		[Fact]
		public void CheckInitialBoard() {
			// Check initial state for player 1
			ChessBoard b = new ChessBoard();
			var index = 0;
			var pieces = GetAllPiecesForPlayer(b, 1).ToList();
			foreach (var pos in GetPositionsInRank(2)) {
				pieces[index].PieceType.ShouldBeEquivalentTo(b.GetPieceAtPosition(pos).PieceType, "Should have been at the initial board state for rank 2 p1");
				index++;
			}
			foreach (var pos in GetPositionsInRank(1)) {
				pieces[index].PieceType.ShouldBeEquivalentTo(b.GetPieceAtPosition(pos).PieceType, "Should have been at the initial board state for rank 1 p1");
				index++;
			}

			// Check initial state for player 2
			index = 0;
			pieces = GetAllPiecesForPlayer(b, 2).ToList();
			foreach (var pos in GetPositionsInRank(8)) {
				pieces[index].PieceType.ShouldBeEquivalentTo(b.GetPieceAtPosition(pos).PieceType, "board should have been at the initial state for rank 8 p2");
				index++;
			}
			foreach (var pos in GetPositionsInRank(7)) {
				pieces[index].PieceType.ShouldBeEquivalentTo(b.GetPieceAtPosition(pos).PieceType, "board should have been at the initial state for rank 7 p2");
				index++;
			}
		}

		[Fact]
		public void CheckBlackPieces() {
			ChessBoard cb = new ChessBoard();

			//checks for correct position of player 2 pawns
			//and placement of black pieces in first row
			for (int i = 0; i < 8; i++) {
				cb.GetPieceAtPosition(Pos(1, i)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at position (1," + i + ")");
				cb.GetPlayerAtPosition(Pos(0, i)).Should().Be(2, "Player 2 has pieces all in row 0");
			}
			//checks the presence of a Player 2 king
			cb.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black has a king at position (0,4)");
		}

		/// <summary>
		/// Check the initial positions of rooks.
		/// </summary>
		[Fact]
		public void CheckInitialRooks() {
			ChessBoard b = new ChessBoard();
			//Test White Rooks' Positions
			b.GetPositionsOfPiece(ChessPieceType.Rook, 1).Should().HaveCount(2, "White's rooks at position (7,0) & (7,7)")
				 .And.BeEquivalentTo(Pos(7, 0), Pos(7, 7));
			//Test Black Rooks' Positions
			b.GetPositionsOfPiece(ChessPieceType.Rook, 2).Should().HaveCount(2, "White's rooks at position (0,0) & (0,7)")
				 .And.BeEquivalentTo(Pos(0, 0), Pos(0, 7));
		}


		[Fact]
		public void InitialRookMoves() {
			ChessBoard b = new ChessBoard();

			// Checks initial white rooks position

			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Rook, "White's left rook at position (0,0)");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Rook, "White's right rook at position (0,7)");

			var possMoves = b.GetPossibleMoves();

			// Checks initial white rooks possible moves
			var whiteRook = GetMovesAtPosition(possMoves, Pos("a1"));
			whiteRook.Should().HaveCount(0, "Left white rook should have 0 possible moves at the start");

			// moves the white pawn in front of the rook up
			Apply(b, "a2, a3");

			// Checks initial Black rooks position
			b.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.Rook, "Black's left rook at position (0,0)");
			b.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.Rook, "Black's right rook at position (0,7)");

			possMoves = b.GetPossibleMoves();
			var blackRook = GetMovesAtPosition(possMoves, Pos("a8"));
			whiteRook.Should().HaveCount(0, "Left Black rook should have 0 possible moves at the start");
		}

		[Fact]
		public void CheckInitialQueensAndKings() {
			ChessBoard board = new ChessBoard();
			var possMoves = board.GetPossibleMoves();
			var blackQueenNoMovesExpected = GetMovesAtPosition(possMoves, Pos("d8")); //initial moves for black queen
			blackQueenNoMovesExpected.Should().HaveCount(0, "the black queen cannot have any moves initally because pawns are in front of it");
			var whiteQueenNoMovesExpected = GetMovesAtPosition(possMoves, Pos("d1")); //initial moves for white queen
			whiteQueenNoMovesExpected.Should().HaveCount(0, "the white queen cannot have any moves initally because pawns are in front of it");

			//Check for 2 kings
			board.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "there should be a king at e8");
			board.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "there should be a king at e1");
		}
	}


}
