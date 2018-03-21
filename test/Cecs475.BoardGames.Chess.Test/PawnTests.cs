using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class PawnTests : ChessTest {
		/// <summary>
		/// Moving pawns one or two spaces.
		/// </summary>
		[Fact]
		public void PawnTwoSpaceMove() {
			ChessBoard b = new ChessBoard();

			var possMoves = b.GetPossibleMoves();
			// Each of the pawns in rank 2 should have two move options.
			foreach (var pos in GetPositionsInRank(2)) {
				var movesAtPos = GetMovesAtPosition(possMoves, pos);
				movesAtPos.Should().HaveCount(2)
					.And.BeEquivalentTo(
						Move(pos, pos.Translate(-1, 0)),
						Move(pos, pos.Translate(-2, 0))
				);
			}
			Apply(b, "a2, a3"); // one space move

			// Same, but for pawns in rank 7
			possMoves = b.GetPossibleMoves();
			foreach (var pos in GetPositionsInRank(7)) {
				var movesAtPos = GetMovesAtPosition(possMoves, pos);
				movesAtPos.Should().HaveCount(2)
					.And.BeEquivalentTo(
						Move(pos, pos.Translate(1, 0)),
						Move(pos, pos.Translate(2, 0))
					);
			}
			Apply(b, "a7, a5"); // player 2 response

			possMoves = b.GetPossibleMoves();
			var oneMoveExpected = GetMovesAtPosition(possMoves, Pos("a3"));
			oneMoveExpected.Should().Contain(Move("a3, a4"))
				.And.HaveCount(1, "a pawn not in its original rank can only move one space forward");

			var twoMovesExpected = GetMovesAtPosition(possMoves, Pos("b2"));
			twoMovesExpected.Should().Contain(Move("b2, b3"))
				.And.Contain(Move("b2, b4"))
				.And.HaveCount(2, "a pawn in its original rank can move up to two spaces forward");
		}

		[Fact]
		public void PawnTwoSpaceMove_IfUnblocked() {
			// Move a pawn from each side to in front of the other's starting pawns.
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b7, b5",
				"a4, a5",
				"b5, b4",
				"a5, a6",
				"b4, b3"
			);

			var possMoves = b.GetPossibleMoves();
			var blockedPawn = GetMovesAtPosition(possMoves, Pos("b2"));
			blockedPawn.Should().BeEmpty("The pawn at b2 is blocked by the enemy at b3");
			Apply(b, Move("c2, c4"));

			possMoves = b.GetPossibleMoves();
			blockedPawn = GetMovesAtPosition(possMoves, Pos("a7"));
			blockedPawn.Should().BeEmpty("The pawn at a7 is blocked by the enemy at a6");
		}

		/// <summary>
		/// A pawn cannot make a two space movement if it is on the enemy's starting rank.
		/// </summary>
		[Fact]
		public void PawnTwoSpaceMove_DirectionMatters() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b7, b5",
				"a4, b5",
				"b8, a6",
				"b5, b6",
				"h7, h5",
				"b6, b7",
				"h5, h4"
			);
			var possMoves = b.GetPossibleMoves();
			var oneMove = GetMovesAtPosition(possMoves, Pos("b7"));
			// The pawn at b7 should have 12 possible moves: 4 promotion moves each, for a move forward,
			// and two capturing moves diagonally.
			oneMove.Should().HaveCount(12)
				.And.NotContain(Move("b7, b9"))
				.And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote,
					"the pawn at b7 should have 12 possible moves, all of which are pawn promotions.");
		}

		/// <summary>
		/// Pawn diagonal capture.
		/// </summary>
		[Fact]
		public void PawnCapture() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b7, b5"
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("a4"));
			expected.Should().Contain(Move("a4, b5"))
				.And.Contain(Move("a4, a5"))
				.And.HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no operations have changed the advantage");

			Apply(b, Move("a4, b5"));
			b.GetPieceAtPosition(Pos("b5")).Player.Should().Be(1, "Player 1 captured Player 2's pawn diagonally");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single pawn of 1 value");

			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "after undoing the pawn capture, advantage is neutral");
		}

		/// <summary>
		/// Pawn capture even if it can't move forward.
		/// </summary>
		[Fact]
		public void PawnCapture_EvenIfBlocked() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"h7, h5",
				"a4, a5",
				"h5, h4",
				"a5, a6",
				"h4, h3"
			);

			var possMoves = b.GetPossibleMoves();
			var threeMoves = GetMovesAtPosition(possMoves, Pos("a6"));
			threeMoves.Should().HaveCount(1, "the pawn at a6 should be able to capture to b7 even if it can't move forward")
				.And.BeEquivalentTo(Move("a6, b7"));
		}

		/// <summary>
		/// Pawn can't capture backwards.
		/// </summary>
		[Fact]
		public void PawnCapture_NoBackwardsCapture() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"b7, b5",
				"a4, a5",
				"c7, c6",
				"a5, a6"
			);

			var possMoves = b.GetPossibleMoves();
			GetMovesAtPosition(possMoves, Pos("b5")).Should().HaveCount(1, "pawn at b5 cannot capture backwards to a6")
				.And.Contain(Move("b5, b4"));
		}

		/// <summary>
		/// Pawns cannot capture diagonally off the board, wrapping to another row.
		/// </summary>
		[Fact]
		public void PawnBorderCapture() {
			ChessBoard b = CreateBoardFromMoves(
				Move("a2, a4"),
				Move("h7, h5")
			);

			var possMoves = b.GetPossibleMoves();
			var forwardOnly = GetMovesAtPosition(possMoves, Pos("a4"));
			forwardOnly.Should().HaveCount(1)
				.And.Contain(Move("a4, a5"));

			Apply(b, Move("b2, b4"));
			possMoves = b.GetPossibleMoves();
			forwardOnly = GetMovesAtPosition(possMoves, Pos("h5"));
			forwardOnly.Should().HaveCount(1, "pawn at h5 can move forward but can't capture to a4")
				.And.Contain(Move("h5, h4"));
		}

		/// <summary>
		/// Promote a pawn after reaching the final rank.
		/// </summary>
		[Fact]
		public void PawnPromoteTest() {
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"a7, a5",
				"b4, b5",
				"a8, a6",
				"b5, a6", // capture rook with pawn
				"b8, c6",
				"a6, a7",
				"c6, d4"
			);
			b.CurrentAdvantage.Should().Be(Advantage(1, 5), "a Black rook was captured");

			// Make sure all possible moves are marked PawnPromote.
			var possMoves = b.GetPossibleMoves();
			var pawnMoves = GetMovesAtPosition(possMoves, Pos("a7"));
			pawnMoves.Should().HaveCount(4, "there are four possible promotion moves")
				.And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote);

			// Apply the promotion move
			Apply(b, Move("(a7, a8, Queen)"));
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Queen, "the pawn was replaced by a queen");
			b.GetPieceAtPosition(Pos("a8")).Player.Should().Be(1, "the queen is controlled by player 1");
			b.CurrentPlayer.Should().Be(2, "choosing the pawn promotion should change the current player");
			b.CurrentAdvantage.Should().Be(Advantage(1, 13), "gained 9 points, lost 1 point from queen promotion");

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "undoing a pawn promotion should change the current player");
			b.CurrentAdvantage.Should().Be(Advantage(1, 5), "lose value of queen when undoing promotion");
		}

		/// <summary>
		/// Promote a pawn to rook, move the rook, ensure that castling is still allowed.
		/// </summary>
		[Fact]
		public void PawnPromote_Castling() {
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"a7, a5",
				"b4, b5",
				"a8, a6",
				"b5, a6", // capture rook with pawn
				"b8, c6",
				"a6, a7",
				"c6, d4"
			);
			b.CurrentAdvantage.Should().Be(Advantage(1, 5), "a Black rook was captured");

			// Apply the promotion move
			Apply(b, Move("(a7, a8, Rook)"));
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "the pawn was replaced by a rook");
			b.GetPieceAtPosition(Pos("a8")).Player.Should().Be(1, "the rook is controlled by player 1");

			Apply(b,
				"h7, h6",
				"a8, b8", // move promoted rook
				"h6, h5",
				"b1, a3", // move white pieces out of the way for castling
				"h5, h4",
				"c1, b2",
				"h4, h3",
				"c2, c3",
				"g7, g6",
				"d1, c2",
				"g6, g5"
			);

			var possMoves = b.GetPossibleMoves();
			var forKing = GetMovesAtPosition(possMoves, Pos("e1"));
			forKing.Should().HaveCount(2, "king at e1 can castle queenside even after a pawn-promoted rook has moved")
				.And.BeEquivalentTo(Move("e1, d1"), Move("e1, c1"));
		}

		/// <summary>
		/// Promote a pawn and produce check.
		/// </summary>
		[Fact]
		public void PawnPromote_IntoCheckmate() {
			ChessBoard b = CreateBoardFromMoves(
				"b2, b4",
				"a7, a5",
				"b4, b5",
				"a8, a6",
				"b5, a6", // capture rook with pawn
				"b8, c6",
				"a6, a7",
				"b7, b5",
				"c2, c3",
				"c8, b7",
				"c3, c4",
				"d7, d6",
				"c4, c5",
				"d8, d7"
			);

			// Apply the promotion move
			Apply(b, Move("(a7, a8, Rook)"));
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Rook, "the pawn was replaced by a rook");
			b.GetPieceAtPosition(Pos("a8")).Player.Should().Be(1, "the rook is controlled by player 1");
			b.IsCheck.Should().BeTrue("the king is threatened by the pawn-promoted rook at a8");
			b.IsCheckmate.Should().BeFalse("the king has an escape");
		}

		[Fact]
		public void EnPassantTest() {
			ChessBoard b = CreateBoardFromMoves(
				"a2, a4",
				"h7, h5",
				"a4, a5"
			);

			// Move pawn forward twice, enabling en passant from a5
			Apply(b, "b7, b5");
			
			var possMoves = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("a5"));
			enPassantExpected.Should().HaveCount(2, "pawn can move forward one or en passant")
				.And.Contain(Move("a5, a6"))
				.And.Contain(Move(Pos("a5"), Pos("b6"), ChessMoveType.EnPassant));

			// Apply the en passant
			Apply(b, Move(Pos("a5"), Pos("b6"), ChessMoveType.EnPassant));
			var pawn = b.GetPieceAtPosition(Pos("b6"));
			pawn.Player.Should().Be(1, "pawn performed en passant move");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);
			var captured = b.GetPieceAtPosition(Pos("b5"));
			captured.Player.Should().Be(0, "the pawn that moved to b5 was captured by en passant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1));

			// Undo the move and check the board state
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0));
			pawn = b.GetPieceAtPosition(Pos("a5"));
			pawn.Player.Should().Be(1);
			captured = b.GetPieceAtPosition(Pos("b5"));
			captured.Player.Should().Be(2);
			var empty = b.GetPieceAtPosition(Pos("b6"));
			empty.Player.Should().Be(0);
		}


		// STUDENT TESTS
		/// <summary>
		/// Tests events when Black Pawn Captures Unmoved Rook or Bishop and
		/// Promotes to Queen at same time
		/// </summary>
		[Fact]
		public void BlackPawnCaptureAndPromotionTest() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 5), new ChessPiece(ChessPieceType.Bishop, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 6), new ChessPiece(ChessPieceType.Knight, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 7), new ChessPiece(ChessPieceType.Rook, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.King, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(6, 6), new ChessPiece(ChessPieceType.Pawn, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			Apply(b, Move("e1, d1"));

			var possMoves = b.GetPossibleMoves();

			var pawnMoves = GetMovesAtPosition(possMoves, Pos("g2"));

			pawnMoves.Should().Contain(Move("g2, h1, Rook"), "Pawn takes Rook (h1) and promotes to Rook")
					  .And.Contain(Move("g2, h1, Bishop"), "Pawn takes Rook (h1) and promotes to Bishop")
					  .And.Contain(Move("g2, h1, Knight"), "Pawn takes Rook (h1) and promotes to Knight")
					  .And.Contain(Move("g2, h1, Queen"), "Pawn takes Rook (h1) and promotes to Queen")
					  .And.Contain(Move("g2, f1, Rook"), "Pawn takes Bishop (f1) and promotes to Rook")
					  .And.Contain(Move("g2, f1, Bishop"), "Pawn takes Bishop (f1) and promotes to Bishop")
					  .And.Contain(Move("g2, f1, Knight"), "Pawn takes Bishop (f1) and promotes to Knight")
					  .And.Contain(Move("g2, f1, Queen"), "Pawn takes Bishop (f1) and promotes to Queen")
					  .And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote, "All Possible Pawn Moves should be promotion only")
					  .And.HaveCount(8, "a pawn can capture the Rook diagonally or Bishop diagonally as well as be promoted 4 different types for each possible capture");

			b.CurrentAdvantage.Should().Be(Advantage(1, 10), "White should be leading with Bishop, Knight and Rook - Black's Pawn");

			Apply(b, Move("(g2, h1, Queen"));

			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Queen, "Pawn should promote to Queen");

			b.CurrentAdvantage.Should().Be(Advantage(2, 3), "Advantage shifts to Player 2 after defeating Rook and promoting pawn to Queen");
		}

		/// <summary>
		/// Performing and undoing Black's Pawn Promotion to and from Queen
		/// </summary>
		[Fact]
		public void BlackPawnUndoPromotion() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 2), new ChessPiece(ChessPieceType.King, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.Rook, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(6, 6), new ChessPiece(ChessPieceType.Pawn, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			Apply(b, Move("e1, d1")); // Change turn to Black
			Apply(b, Move("(g2, g1, Queen)")); // Pawn promotion to Queen

			b.CurrentAdvantage.Should().Be(Advantage(2, 4), "Advantage shifts to Black Player for promoting to Queen");

			// Undos Black promotion from Pawn to Queen
			b.UndoLastMove();

			b.CurrentAdvantage.Should().Be(Advantage(1, 4), "Advantage shifts back to White when Promotion is undone");
		}

		[Fact]
		public void BlackEnPassant() {
			ChessBoard board = CreateBoardFromMoves(
				"d2, d4",
				"h7, h6",
				"d4, d5",
				"e7, e5",
				"d5, e6"
			);
			board.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at position e6");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "Black's pawn taken at position e5");
			board.GetPieceAtPosition(Pos("e6")).Player.Should().Be(1, "Player 1 captured Player 2's pawn diagonally");
			board.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single pawn of 1 value");
		}

		[Fact]
		public void BlackUndo() {
			ChessBoard board = CreateBoardFromMoves(
				"e2, e4",
				"e7, e5",
				"g1, f3",
				"b8, c6",
				"d2, d4",
				"g8, f6",
				"d4, e5",
				"g7, g6",
				"e5, f6"
			);

			board.GetPieceAtPosition(Pos("f6")).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn took Black's Knight");
			board.GetPieceAtPosition(Pos("f6")).Player.Should().Be(1, "The pawn at f6 belongs to White");
			board.CurrentAdvantage.Should().Be(Advantage(1, 4), "White has a one knight plus a one pawn advantage advantage");

			board.UndoLastMove();

			board.GetPieceAtPosition(Pos("f6")).PieceType.Should().Be(ChessPieceType.Knight, "Black's knight is back at position f6");
			board.GetPieceAtPosition(Pos("f6")).Player.Should().Be(2, "The knight at f6 belongs to Black");
			board.CurrentAdvantage.Should().Be(Advantage(1, 1), "White has a one pawn advantage on the board");
		}

		[Fact]
		public void UndoPromotionShouldReturnPawn() {
			ChessBoard b = CreateBoardFromPositions(
			Pos("a2"), ChessPieceType.Pawn, 2,//black
			Pos("e1"), ChessPieceType.King, 1,//white
			Pos("e8"), ChessPieceType.King, 2//black
			);
			Apply(b, "e1, e2");
			Apply(b, "a2, a1, Queen");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Pawn, "the piece should revert to being a Pawn after undoing the move. ");
		}

		[Fact]
		public void EnPassantUndo() {
			ChessBoard b = new ChessBoard();
			b.ApplyMove(Move(Pos("a2"), Pos("a4")));
			b.UndoLastMove();
			var possMoves = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("a2"));
			enPassantExpected.Should().HaveCount(2, "pawn can move forward one or en passant even if you undo enpassant once")
				 .And.Contain(Move("a2, a3"))
				 .And.Contain(Move(Pos("a2"), Pos("a4"), ChessMoveType.EnPassant));
		}

		/// <summary>
		/// Test Tricky Moves
		/// </summary>

		[Fact]
		public void TestTrickyMovePawnPromotion() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("h8"), ChessPieceType.King, 2,
				 Pos("b7"), ChessPieceType.Pawn, 1,
				 Pos("c2"), ChessPieceType.Pawn, 2
			);

			b.GetPieceAtPosition(Pos("b7")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 1), "Position b7 should be a Pawn for player 1");
			Apply(b, Move("(b7, b8, Queen)"));
			b.GetPieceAtPosition(Pos("b8")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Queen, 1), "Position b8 should be a Queen for player 1");

		}

		/// <summary>
		/// Undo and redo EnPassant
		/// </summary>
		[Fact]
		public void EnPassantUndoAndRedo() {
			ChessBoard b = CreateBoardFromMoves(
				 "b2, b4",
				 "f7, f5",
				 "b4, b5",
				 "a7, a5"
			);
			b.GetPieceAtPosition(Pos("b5")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 1), "Position b5 should be a Pawn for player 1");
			b.GetPieceAtPosition(Pos("a5")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 2), "Position a5 should be a Pawn for player 2");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Black has a current advantage 0");

			// Do EnPassant
			Apply(b, Move("b5, a6"));

			b.GetPieceAtPosition(Pos("a6")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 1), "Position a6 should be a Pawn for player 1 after EnPassant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single pawn of 1 value");

			// Undo EnPassant
			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("b5")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 1), "Position b5 should be a Pawn for player 1");
			b.GetPieceAtPosition(Pos("a5")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 2), "Position a5 should be a Pawn for player 2");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Black returned back to current advantage of 0");

			// Redo EnPassant
			Apply(b, Move("b5, a6"));
			b.GetPieceAtPosition(Pos("a6")).ShouldBeEquivalentTo(new ChessPiece(ChessPieceType.Pawn, 1), "Position a6 should be a Pawn for player 1 after redo EnPassant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single pawn of 1 value again");
		}

		[Fact]
		public void PromotePawnToRook() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "b7, b5",
				 "a1, a3",
				 "b5, a4",
				 "a3, b3",
				 "a4, a3",
				 "b3, b4",
				 "a3, a2",
				 "b4, b5"
			);
			Apply(b, Move("a2, a1, Rook"));

			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "pawn should promote to Rook");

		}

		/// <summary>
		/// The test check when is en passant move is valid
		/// Test : - "Tricky" test with En Passant Test
		/// Player: - Black,White
		/// Result: - En passant is only possible if the pawn being captured moved two sqaures forward in it's recent turn. Or else,
		/// it is not possible
		/// </summary>
		[Fact]
		public void EnPassantTest2() {
			ChessBoard board = CreateBoardFromMoves(
				 "h2,h4",
				 "b7,b5",
				 "h4,h5",
				 "b5,b4",
				 "a2,a4",
				 "g7,g5");

			var possibleMoves = board.GetPossibleMoves();
			var whitePawnPassant = GetMovesAtPosition(possibleMoves, Pos("h5"));

			whitePawnPassant.Should().Contain(Move(Pos("h5"), Pos("g6"), ChessMoveType.EnPassant), "The white pawn should be able to " +
				 "capture black pawn at g5");

			//Apply the en passant move
			Apply(board, Move(Pos("h5"), Pos("g6"), ChessMoveType.EnPassant));

			var emptySquare = board.GetPieceAtPosition(Pos("g5"));
			emptySquare.Player.Should().Be(0, "The black pawn previously at g5 should be captured.");

			possibleMoves = board.GetPossibleMoves();
			var blackPawnPassant = GetMovesAtPosition(possibleMoves, Pos("b4"));

			blackPawnPassant.Should().NotContain(Move(Pos("b4"), Pos("a3"), ChessMoveType.EnPassant), "The black pawn cannot capture the" +
				 "white pawn with en passant rule because the move was made by white two moves ago.");

		}

		[Fact]
		public void PerformEnPassantOnlyOnFirstChance() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "h7, h6",
				 "a4, a5",
				 "b7, b5"
			);

			//check for pawn an at a5
			//should have two moves
			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("a5"));
			expected.Should().Contain(Move("a5, a6"))
				 .And.Contain(Move("a5, b6"))
				 .And.HaveCount(2, "a pawn can only move forward one space after its first move, "
									  + "and can only capture pieces diagonal infront of it "
									  + "but can perform en passant on a pawn if its on its fifth rank");

			Apply(b, Move("b2, b3"));
			Apply(b, Move("g7, g6"));

			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("a5"));
			expected.Should().Contain(Move("a5, a6"))
				 .And.HaveCount(1, "En Passant can only be performed directly after the opponent "
									  + "moved his pawn two spaces from its starting position");

		}

		/// <summary>
		/// At least 2 tests must involve Undo Last Move.
		/// </summary>
		[Fact]
		public void undoEnPassant() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4",
				 "a7, a6",
				 "a4, a5",
				 "b7, b5"
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("a5"));
			expected.Should().Contain(Move("a5, b6"))
				 .And.HaveCount(1, "a pawn can capture an enemy pawn on its fifth rank "
									  + "if it had just performed a space jump");

			Apply(b, Move("a5, b6"));

			b.UndoLastMove();
			b.UndoLastMove();

			Apply(b, Move("b7, b5"));

			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("a5"));
			expected.Should().Contain(Move("a5, b6"))
				 .And.HaveCount(1, "a pawn can capture an enemy pawn on its fifth rank "
									  + "if it had just performed a space jump, even if the moves were reset");
		}

		/// <summary>
		/// Testing enpassant
		/// </summary>
		[Fact]
		public void EnPassant() {
			ChessBoard b = CreateBoardFromMoves(
				"c2, c4",
				"f7, f6",
				"c4, c5",
				"d7, d5"
			);

			var possMoves = b.GetPossibleMoves();
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("c5"));
			enPassantExpected.Should().HaveCount(2, "pawn can move forward one space or capture en passant")
				.And.Contain(Move("c5, c6"))
				.And.Contain(Move(Pos("c5"), Pos("d6"), ChessMoveType.EnPassant));

			// Apply the en passant
			Apply(b, Move(Pos("c5"), Pos("d6"), ChessMoveType.EnPassant));
			var pawn = b.GetPieceAtPosition(Pos("d6"));
			pawn.Player.Should().Be(1, "pawn performed en passant move");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn, "pawn performed en passant move");
			var captured = b.GetPieceAtPosition(Pos("d5"));
			captured.Player.Should().Be(0, "the pawn that moved to b5 was captured by en passant");
			captured.PieceType.Should().Be(ChessPieceType.Empty, "the pawn that moved to b5 was captured by en passant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "player 1 captured player 2's pawn by en passant");

			// Undo the move and check the board state
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "en passant move was undone");
			pawn = b.GetPieceAtPosition(Pos("c5"));
			pawn.Player.Should().Be(1, "en passant move was undone");
			captured = b.GetPieceAtPosition(Pos("d5"));
			captured.Player.Should().Be(2, "en passant move was undone");
			var empty = b.GetPieceAtPosition(Pos("d6"));
			empty.Player.Should().Be(0, "neither player has a piece at d6");
			empty.PieceType.Should().Be(ChessPieceType.Empty, "en passant move was undone");
		}

		/// <summary>
		/// Very simple undo after one move
		/// </summary>
		[Fact]
		public void CheckSimpleUndo() {
			ChessBoard b = new ChessBoard();
			var pieces = GetAllPiecesForPlayer(b, 1);
			var possMoves = b.GetPossibleMoves();
			Apply(b, "a2, a3"); // one space move from p1
			b.PositionIsEmpty(Pos("a2")).Should().BeTrue("previous piece position should be empty after a move");
			b.PositionIsEmpty(Pos("a3")).Should().BeFalse("new piece position on board should contain the moved piece");
			b.UndoLastMove();
			b.PositionIsEmpty(Pos("a3")).Should().BeTrue("previous piece position should contain piece again after undo a move");
			b.PositionIsEmpty(Pos("a2")).Should().BeFalse("new piece position on board should not contain the moved piece anymore after an undo");
		}

		/// <summary>
		/// Check pawn promotion to a queen + En Passant move
		/// </summary>
		[Fact]
		public void CheckPawnPromotionAndEnPassant() {
			ChessBoard b = new ChessBoard();

			Apply(b, "a2, a4"); // Move p1's pawn
			Apply(b, "b7, b5"); // Move p2's pawn
			Apply(b, "a4, b5"); // Take p2's pawn with p1's pawn

			Apply(b, "c7, c5"); // Move p2's pawn to make a en passant situation
			var pawnMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("b5")); // Check if possible move en passant
			pawnMoves.Should().HaveCount(2, "there is only 2 possible moves for p1's pawn at this stage of game (en passant).").And.Contain(m => m.MoveType == ChessMoveType.EnPassant);
			Apply(b, "b5, c6");
			b.PositionIsEmpty(Pos("c5")).Should().BeTrue("En Passant from p1's pawn should have take the p2's pawn");
			b.PositionIsEmpty(Pos("c6")).Should().BeFalse("p1's pawn should be here after En Passant's move");

			Apply(b, "c8, b7"); // Move p2's bishop
			pawnMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c6"));
			pawnMoves.Should().HaveCount(3, "there is only 3 possible moves for p1's pawn at this stage of game.").And.Contain(m => m.MoveType == ChessMoveType.Normal);
			Apply(b, "c6, c7");
			Apply(b, "h7, h5");

			pawnMoves = GetMovesAtPosition(b.GetPossibleMoves(), Pos("c7"));
			pawnMoves.Should().HaveCount(12, "there is only 12 possible moves for p1's pawn at this stage of game.").And.Contain(m => m.MoveType == ChessMoveType.PawnPromote);
			// Apply the promotion move
			Apply(b, Move("(c7, c8, Queen)"));
			b.GetPieceAtPosition(Pos("c8")).PieceType.ShouldBeEquivalentTo(ChessPieceType.Queen, "p1's pawn should have been promoted to queen");
		}

		/// <summary>
		/// White performs En Passant on Black, advantage is checked,
		/// the move is undone and the advantage is checked again.
		/// </summary>
		[Fact]
		public void UndoEnPassantAdvantage() {
			ChessBoard b = CreateBoardFromMoves(
				 ("d2, d4"),
				 ("g7, g6"),
				 ("d4, d5"),
				 ("e7, e5")
			);

			Apply(b, Move("d5, e6"));
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "white performed en passant and gained a one point advantage");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "neither player has an advantage after en passant was undone");
		}

		/// <summary>
		/// Black pawn gets promoted to bishop, the move is undone,
		/// black pawn reapplies promotion, but promotes to queen this time
		/// </summary>
		[Fact]
		public void PawnPromotionUndoAndSwap() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("c2"), ChessPieceType.Pawn, 2,
				 Pos("f2"), ChessPieceType.Pawn, 1
			);

			Apply(b, Move("f2, f3"), Move("c2, c1, bishop"));
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "piece should be a bishop after pawn promotion");
			b.UndoLastMove();
			Apply(b, Move("c2, c1, queen"));
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Queen, "piece should be a queen after pawn promotion");
			b.IsCheck.Should().Be(true, "white's king is in check");
		}

		/// <summary>
		/// The purpose of this test is to test promotion
		/// and if you can undo/redo it many times 
		/// while keeping a correct game advantage
		/// </summary>
		[Fact]
		public void PromotionUndoRedo() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a2"), ChessPieceType.Knight, 1,
				 Pos("h7"), ChessPieceType.Pawn, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("g8"), ChessPieceType.Knight, 2,
				 Pos("a7"), ChessPieceType.Pawn, 2
			);
			var possMoves = b.GetPossibleMoves();
			var pawnToPromote = GetMovesAtPosition(possMoves, Pos("h7"));
			pawnToPromote.Should().HaveCount(8, "pawn can promote 2 times to 4 different pieces")
				 .And.NotContain(Move("h7, h9"))
				 .And.NotContain(Move("h7, i8"))
				 .And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote,
					  "the only possible moves should be promotions");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no advantage yet");


			Apply(b, "h7, h8, Rook");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "Pawn should have promoted to Rook in h8");
			b.GetPieceAtPosition(Pos("h7")).PieceType.Should().Be(ChessPieceType.Empty, "Pawn should have been removed in h7");
			b.CurrentAdvantage.Should().Be(Advantage(1, 4), "-1 Pawn +5 Rook = +4 advantage");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Empty, "undo the promotion to rook");
			b.GetPieceAtPosition(Pos("h7")).PieceType.Should().Be(ChessPieceType.Pawn, "get back the pawn in h7");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "advantage should be back to 0");

			Apply(b, "h7, g8, Queen");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Queen, "Pawn should have promoted to Queen in g8");
			b.GetPieceAtPosition(Pos("h7")).PieceType.Should().Be(ChessPieceType.Empty, "Pawn should have been removed in h7");
			b.CurrentAdvantage.Should().Be(Advantage(1, 11), "-1 Pawn +9 Queen -3 Knight = +11 advantage");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight, "undo pawn take knight and promote so get back a knight");
			b.GetPieceAtPosition(Pos("h7")).PieceType.Should().Be(ChessPieceType.Pawn, "get back the pawn in h7");
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "advantage should be back to 0");
		}

		/// <summary>
		/// The purpose of this test is to test that you can take en passant and the 
		/// undo restore the taken pawn at the good place
		/// </summary>
		[Fact]
		public void WhiteEnPassantPlusUndo() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("d1"), ChessPieceType.Queen, 1,
				 Pos("d5"), ChessPieceType.Pawn, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("d8"), ChessPieceType.Queen, 2,
				 Pos("e7"), ChessPieceType.Pawn, 2
			);
			Apply(b,
				 "d1, c1",
				 "e7, e5");
			var possMoves = b.GetPossibleMoves();
			var enPassantPawn = GetMovesAtPosition(possMoves, Pos("d5"));
			enPassantPawn.Should().HaveCount(2, "pawn can move forward to d6 or en passant to e6")
				 .And.Contain(Move("d5, d6"))
				 .And.Contain(Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "no advantage yet");
			Apply(b, "d5, e6");
			b.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be at e6");
			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Empty, "Should be empty due to en passant");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Lose value of Pawn by en passant");
			b.UndoLastMove();
			b.CurrentAdvantage.Should().Be(Advantage(0, 0), "Undo move so advantage should back to 0");
			b.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be back to d5 after undo");
			b.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be back to e5 after undo");
		}

		/// <summary>
		/// The purpose of this test is to test that you can only take en passant when
		/// the ennemy pawn just moved and not the next turn
		/// </summary>
		[Fact]
		public void WhiteEnPassantTooLate() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("d4"), ChessPieceType.Pawn, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e7"), ChessPieceType.Pawn, 2
			);

			Apply(b,
				 "d4, d5",
				 "e7, e5");
			var possMoves = b.GetPossibleMoves();
			var enPassantPawn = GetMovesAtPosition(possMoves, Pos("d5"));
			enPassantPawn.Should().HaveCount(2, "pawn can move forward to d6 or en passant to e6")
				 .And.Contain(Move("d5, d6"))
				 .And.Contain(Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant));
			Apply(b,
				 "e1, f1",
				 "e8, f8");
			possMoves = b.GetPossibleMoves();
			enPassantPawn = GetMovesAtPosition(possMoves, Pos("d5"));
			enPassantPawn.Should().HaveCount(1, "pawn can only move forward too late to en passant")
				 .And.Contain(Move("d5, d6"));
		}

		/// <summary>
		/// The purpose of this test is to test that you can't take en passant if it leaves your king in check
		/// </summary>
		[Fact]
		public void BlackImpossibleEnPassantToCheck() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("g1"), ChessPieceType.King, 1,
				 Pos("g2"), ChessPieceType.Pawn, 1,
				 Pos("h1"), ChessPieceType.Rook, 1,
				 Pos("g8"), ChessPieceType.King, 2,
				 Pos("h4"), ChessPieceType.Pawn, 2
			);
			Apply(b,
				 "g2, g4");
			var possMoves = b.GetPossibleMoves();
			var enPassantPawn = GetMovesAtPosition(possMoves, Pos("h4"));
			enPassantPawn.Should().HaveCount(2, "pawn can move forward to h3 or en passant to g3")
				 .And.Contain(Move("h4, h3"))
				 .And.Contain(Move(Pos("h4"), Pos("g3"), ChessMoveType.EnPassant));
			b.UndoLastMove();
			Apply(b,
				 "g1, f1",
				 "g8, h8",
				 "g2, g4");
			possMoves = b.GetPossibleMoves();
			enPassantPawn = GetMovesAtPosition(possMoves, Pos("h4"));
			enPassantPawn.Should().HaveCount(1, "en passant is illegal since it would let king in check")
				 .And.Contain(Move("h4, h3"));
		}

		///<summary>
		/// Promotes a white pawn to a queen
		///</summary>
		[Fact]
		public void WhitePawnPromotion() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 4), ChessPieceType.Pawn, 1,  //white pawn to promote
				 Pos(2, 2), ChessPieceType.Knight, 2,
				 Pos(2, 6), ChessPieceType.King, 2,
				 Pos(6, 4), ChessPieceType.King, 1);

			// Make sure all possible moves are marked PawnPromote.
			var possMoves = cb.GetPossibleMoves();
			var pawnMoves = GetMovesAtPosition(possMoves, Pos(1, 4));
			pawnMoves.Should().HaveCount(4, "there are four possible promotion moves")
				 .And.OnlyContain(m => m.MoveType == ChessMoveType.PawnPromote);

			// Apply the promotion move
			Apply(cb, Move("(e7, e8, Queen)"));
			cb.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.Queen, "P1 Pawn is now a Queen");
			cb.GetPieceAtPosition(Pos(0, 4)).Player.Should().Be(1, "Current Queen: P1");
			cb.CurrentPlayer.Should().Be(2, "Switch to P2 after Pawn Promotion");

			cb.UndoLastMove();
			cb.CurrentPlayer.Should().Be(1, "Change to P1");
		}

		/// <summary>
		/// Pawn move forward 2 spaces, undo it, check if it can move forward 2 spaces
		/// </summary>
		[Fact]
		public void UndoFirstPawn() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a4"
			);
			//Check Black Pawn's possible moves
			var possMoves = b.GetPossibleMoves();
			var forPawn = GetMovesAtPosition(possMoves, Pos("f7"));
			forPawn.Should().HaveCount(2, "Black Pawn can move forward 1 or 2 squares")
				 .And.BeEquivalentTo(Move("f7, f6"), Move("f7, f5"));

			//Move Black Pawn forward
			Apply(b, "f7, f5");
			var pawn = b.GetPieceAtPosition(Pos("f5"));
			pawn.Player.Should().Be(2, "Black pawn move forward 2 spaces");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);

			//Undo Last Move
			b.UndoLastMove();
			pawn = b.GetPieceAtPosition(Pos("f7"));
			pawn.Player.Should().Be(2, "Black pawn return to original spot");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);

			//Check Black Pawn's possible moves again
			forPawn.Should().HaveCount(2, "Black Pawn can move forward 1 or 2 squares")
				 .And.BeEquivalentTo(Move("f7, f6"), Move("f7, f5"));
		}

		[Fact]
		public void CheckEnPassat() {
			ChessBoard b = CreateBoardFromMoves(
					"d2, d4",
					"g7, g5",
					"d4, d5");

			Apply(b, "e7, e5");
			// white is doing the enpassat
			var possMoves = b.GetPossibleMoves();

			var whitePawn = GetMovesAtPosition(possMoves, Pos("d5"));

			whitePawn.Should().Contain(Move(Pos("d5"), Pos("e6"), ChessMoveType.EnPassant))
			.And.Contain(Move("d5, d6")).And.HaveCount(2, "White Left pawn should have 2 possible moves; forward and an EnPassant");

			Apply(b, "f2, f4");
			Apply(b, "f7, f5");

			possMoves = b.GetPossibleMoves();
			whitePawn = GetMovesAtPosition(possMoves, Pos("d5"));

			whitePawn.Should().Contain(Move("d5, d6")).
			And.HaveCount(1, "White Left pawn should have 1 possible moves after not going through with the EnPassant");
		}

		[Fact]
		public void RedoPromotion() {
			ChessBoard b = CreateBoardFromPositions(
						 Pos("h2"), ChessPieceType.Pawn, 2,
						 Pos("g2"), ChessPieceType.Pawn, 1,
						 Pos("b6"), ChessPieceType.King, 2,
						 Pos("g4"), ChessPieceType.King, 1
					);

			// White moves
			Apply(b, "g2, g3");

			// Promotes a black pawn to a queen
			Apply(b, Move("(h2, h1, Queen)"));

			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Queen, "The pawn was promoted to a Queen");
			b.UndoLastMove();

			// Undo the promotion so that the new queen is back to a pawn
			b.GetPieceAtPosition(Pos("h2")).PieceType.Should().Be(ChessPieceType.Pawn, "The pawn was demoted back to a pawn");

			// Have a promotion from pawn to bishop 
			Apply(b, Move("(h2, h1, Bishop)"));
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.Bishop, "The pawn was promoted to a Bishop instead");
		}

		/*
		5. Check for En Passant
		*/
		[Fact]
		public void EnPassantCheck() {
			ChessBoard board = CreateBoardFromMoves(
				"a2, a4",
				"e7, e5",
				"b2, b3",
				"e5, e4",
				"d2, d4" //en passant situation
			);

			//Check number of moves in pawn at e4
			var possMoves = board.GetPossibleMoves();
			var expectedMovesForPawn = GetMovesAtPosition(possMoves, Pos("e4"));
			expectedMovesForPawn.Should().HaveCount(2, "pawn at e4 can move to d3 for capture or e4")
			.And.Contain(Move(Pos("e4"), Pos("d3"), ChessMoveType.EnPassant)).And.Contain(Move("e4, e3"));

			//Apply EnPassant
			Apply(board, Move(Pos("e4"), Pos("d3"), ChessMoveType.EnPassant));

			//d4 and e4 should be empty
			board.PositionIsEmpty(Pos("e4")).Should().BeTrue("the pawn e4 moved to d3 by en passant");
			board.PositionIsEmpty(Pos("d4")).Should().BeTrue("the pawn at d4 was captured");
			board.PositionIsEmpty(Pos("d3")).Should().BeFalse("the pawn moved from e4 to d3 after en passant move");
			board.CurrentAdvantage.Should().Be(Advantage(2, 1), "black got 1 point for capturing the pawn");
		}
	}
}
