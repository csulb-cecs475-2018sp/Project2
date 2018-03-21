using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class KingTests : ChessTest {
		
		[Fact]
		public void BlackKingNoMoves() {
			ChessBoard board = CreateBoardFromMoves(
				"b2, b3",
				"e7, e5",
				"c1, a3",
				"c7, c6",
				"g2, g3",
				"d8, b6",
				"f1, h3",
				"d7, d5",
				"d2, d3",
				"b8, a6",
				"d1, d2",
				"g8, h6",
				"d2, g5"
			);
			var possibleMoves = board.GetPossibleMoves();
			var blackKingPossibleMoves = GetMovesAtPosition(possibleMoves, Pos("e8"));
			blackKingPossibleMoves.Should().HaveCount(0, "the king will be attacked if he moves");
		}

		[Fact]
		public void NoCastlingIfKingMoved() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a1"), ChessPieceType.Rook, 1,//white
				 Pos("e8"), ChessPieceType.King, 2,//black
				 Pos("e1"), ChessPieceType.King, 1//white
			);
			Apply(b, "e1, e2");
			Apply(b, "e8, e7");
			Apply(b, "e2, e1");
			Apply(b, "e7, e8");
			var movesPossible = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(movesPossible, Pos("e1"));
			kingMoves.Should().HaveCount(5, "king at e1 cannot castle queenside after the king has already moved even if the king moves back.")
				 .And.BeEquivalentTo(Move("e1, e2"), Move("e1, d1"), Move("e1, d2"), Move("e1, f1"), Move("e1, f2"));

		}

		[Fact]
		public void CannotCastleIntoDanger() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a1"), ChessPieceType.Rook, 1,//white
				 Pos("e8"), ChessPieceType.King, 2,//black
				 Pos("e1"), ChessPieceType.King, 1,//white      
				 Pos("c8"), ChessPieceType.Queen, 2//black
		  );
			var movesPossible = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(movesPossible, Pos("e1"));
			kingMoves.Should().HaveCount(5, "king at e8 cannot castle into danger.")
				 .And.BeEquivalentTo(Move("e1, e2"), Move("e1, d1"), Move("e1, d2"), Move("e1, f1"), Move("e1, f2"));
		}

		[Fact]
		public void UndoCastlingShouldReturnBothPieces() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a1"), ChessPieceType.Rook, 1,//white
				 Pos("e8"), ChessPieceType.King, 2,//black
				 Pos("e1"), ChessPieceType.King, 1//white                                                                                                                                                                                                                                                            
		  );
			Apply(b, "e1, c1");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "the piece should revert to being a Pawn after undoing the move. ");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "the piece should revert to being a Pawn after undoing the move. ");
		}

		[Fact] //w
		public void KingPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "e7, e5",
				 "e1, e2",
				 "d7, d5",
				 "e2, e3",
				 "f7, f5"
			);

			b.GetPieceAtPosition(Pos("e3")).PieceType.Should().Be(ChessPieceType.King, "King should be at position e3");

			var possMoves = b.GetPossibleMoves();
			var threeMoves = GetMovesAtPosition(possMoves, Pos("e3"));
			threeMoves.Should().HaveCount(3, "King has 8 possible moves from location e3");

		}

		/// <summary>
		/// Check Player 1 can castle undo last move then castle again on the opposite side
		/// </summary>
		[Fact]
		public void undo_Castling() {

			ChessBoard b = CreateBoardFromPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("a1"), ChessPieceType.Rook, 1,
				Pos("h1"), ChessPieceType.Rook, 1
			);

			var possMoves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			kingMoves.Should().HaveCount(7, "all regular moves plus castling both king and queen side")
				.And.BeEquivalentTo(Move("e1, d1"), Move("e1, d2"), Move("e1, e2"), Move("e1, f1"), Move("e1, f2"), Move("e1, g1"), Move("e1, c1"));

			Apply(b, Move("(e1, g1)")); // White player castle king side
			b.UndoLastMove();

			b.CurrentPlayer.Should().Be(1, "Player 1 turn after undoing player 1 turn");
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "White's King at position (7,4)");
			b.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.Rook, "White's Rook at position (7,7)");

			possMoves = b.GetPossibleMoves();
			kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			kingMoves.Should().HaveCount(7, "King should still be able to castle after undoing last move")
				.And.BeEquivalentTo(Move("e1, d1"), Move("e1, d2"), Move("e1, e2"), Move("e1, f1"), Move("e1, f2"), Move("e1, g1"), Move("e1, c1"));

			Apply(b, Move("(e1, c1)")); // White player castle queen side

		}

		/// <summary>
		/// White queenside castle
		/// </summary>
		[Fact]
		public void CastleQueenSide() {
			ChessBoard b = CreateBoardFromMoves(
				 Move("d2, d4"),
				 Move("d7, d5"),
				 Move("d1, d3"),
				 Move("d8, d6"),
				 Move("b1, a3"),
				 Move("h7, h6")
				 );

			var possMoves = b.GetPossibleMoves();

			// check if king is able to castle if pieces are in the way
			var castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if there are pieces in the way");

			Apply(b, "c1, d2");
			Apply(b, "d6, a3");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().Contain(Move("e1, c1"))
				 .And.Contain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is able to castle");

			Apply(b, "a1, b1"); // move rook
			Apply(b, "h6, h5");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if the rook has moved");

			Apply(b, "b1, a1"); // rook moves back to original position
			Apply(b, "h5, h4");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if the rook has moved");

			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "undoing a move by multiple of 2 should not change players");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle after undoing rook moves
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().Contain(Move("e1, c1"))
				 .And.Contain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is able to castle");

			Apply(b, "e1, d1"); // move the king
			Apply(b, "h6, h5");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("d1"));
			castlingMoves.Should().NotContain((m => m.MoveType == ChessMoveType.CastleQueenSide), "king is not able to castle if the king has moved");

			Apply(b, "d1, e1"); // move the king back to original position
			Apply(b, "h5, h4");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if the rook has moved");

			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();

			Apply(b, "b2, b3");
			Apply(b, "h6, h5");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if the king will end up in check");

			Apply(b, "d2, e3");
			Apply(b, "a3, b3");
			Apply(b, "h2, h3");
			Apply(b, "b3, d3");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().NotContain(Move("e1, c1"))
				 .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide), "king is not able to castle if the kill will 'go through' a check");
		}

		/// <summary>
		/// Undo white queenside castle
		/// </summary>
		[Fact]
		public void UndoCastleQueenSide() {
			ChessBoard b = CreateBoardFromMoves(
				 Move("d2, d4"),
				 Move("d7, d5"),
				 Move("d1, d3"),
				 Move("d8, d6"),
				 Move("b1, a3"),
				 Move("h7, h6"),
				 Move("c1, d2"),
				 Move("d6, a3"),
				 Move("e1, c1"),
				 Move("h6, h5")
				 );

			var possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			var castlingMoves = GetMovesAtPosition(possMoves, Pos("c1"));
			castlingMoves.Should().NotContain((m => m.MoveType == ChessMoveType.CastleQueenSide), "king is not able to castle if the king has castled");

			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("c1")).Player.Should().Be(1, "piece is white");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("d1")).Player.Should().Be(1, "piece is white");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.UndoLastMove();
			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("e1")).Player.Should().Be(1, "piece is white");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("a1")).Player.Should().Be(1, "piece is white");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle again
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingMoves.Should().Contain(Move("e1, c1"))
				 .And.Contain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleKingSide), "king is able to castle");
		}

		/// <summary>
		/// Undo black kingside castle
		/// </summary>
		[Fact]
		public void UndoCastleKingSide() {
			ChessBoard b = CreateBoardFromMoves(
				 Move("d2, d4"),
				 Move("g7, g6"),
				 Move("d1, d3"),
				 Move("g8, f6"),
				 Move("a2, a3"),
				 Move("f8, h6"),
				 Move("a3, a4"),
				 Move("e8, g8"),
				 Move("a4, a5")
				 );

			var possMoves = b.GetPossibleMoves();

			// check if king is able to castle
			var castlingMoves = GetMovesAtPosition(possMoves, Pos("g8"));
			castlingMoves.Should().NotContain((m => m.MoveType == ChessMoveType.CastleKingSide), "king is not able to castle if the king has castled");

			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("g8")).Player.Should().Be(2, "piece is black");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("f8")).Player.Should().Be(2, "piece is black");
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");

			b.UndoLastMove();
			b.UndoLastMove();

			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Empty, "space is Empty");
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "piece is a King");
			b.GetPieceAtPosition(Pos("e8")).Player.Should().Be(2, "piece is black");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Rook, "piece is a Rook");
			b.GetPieceAtPosition(Pos("h8")).Player.Should().Be(2, "piece is black");

			// recalculate possible moves
			possMoves = b.GetPossibleMoves();

			// check if king is able to castle again
			castlingMoves = GetMovesAtPosition(possMoves, Pos("e8"));
			castlingMoves.Should().Contain(Move("e8, g8"))
				 .And.Contain(Move(Pos("e8"), Pos("g8"), ChessMoveType.CastleKingSide), "king is able to castle");
		}

		/// <summary>
		/// The test checks possible moves for a king at check. 
		/// Test : - KingAtCheck
		/// Player: - Black
		/// Result: - In the given layout of the board, the king cannot capture the pawn that is checking it, as it would 
		/// lead to another check. The king cannot also do castling because it is at check. In addition, the black queen cannot 
		/// capture the white bishop because the king is at check. 
		/// </summary>
		[Fact]
		public void KingCapturePawnAtCheck() {
			ChessBoard board = CreateBoardFromPositions(
				Pos("e1"), ChessPieceType.King, 1,
				Pos("a4"), ChessPieceType.Bishop, 1,
				Pos("c6"), ChessPieceType.Pawn, 1,
				Pos("d7"), ChessPieceType.Pawn, 2,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("f4"), ChessPieceType.Queen, 2,
				Pos("h8"), ChessPieceType.Rook, 2);

			//apply move to the white king
			Apply(board, "e1,d1");

			var possibleMoves = board.GetPossibleMoves();

			//all the moves of black king
			var blackKingMoves = GetMovesAtPosition(possibleMoves, Pos("e8"));

			board.IsCheck.Should().BeFalse("The black king has not been checked yet.");
			blackKingMoves.Should().HaveCount(5, "The black king can move to adjacent squares not occupied by friendly piece" +
				 "as well as the castling move")
				 .And.Contain(Move("e8,d8"))
				 .And.Contain(Move("e8,f8"))
				 .And.Contain(Move("e8,f7"))
				 .And.Contain(Move("e8,e7"))
				 .And.Contain(Move(Pos("e8"), Pos("g8"), ChessMoveType.CastleKingSide));

			//Move the white queen
			Apply(board, "f4,e4");
			//Move the white pawn to capture black pawn
			Apply(board, "c6,d7");

			possibleMoves = board.GetPossibleMoves();

			board.IsCheck.Should().BeTrue("The black king is at check by the white pawn");

			//all the moves valid for the black king at check
			var checkedKingMoves = GetMovesAtPosition(possibleMoves, Pos("e8"));
			//all the moves valid for the black queen
			var queenMoves = GetMovesAtPosition(possibleMoves, Pos("f4"));

			queenMoves.Should().HaveCount(0, "The king is at check so the queen should not be able to move unless" +
				 "the move removes the check")
				 .And.NotContain(Move("f4,a4"));

			checkedKingMoves.Should().HaveCount(4, "The black king can move to adjacent squares but cannot do castling " +
				 "or capture the black pawn")
				 .And.NotContain(Move(Pos("e8"), Pos("g8"), ChessMoveType.CastleKingSide))
				 .And.NotContain(Move("e8,d7"));
		}
		/// <summary>
		/// One test must involve validating Get Possible Moves to see that a particular piece has
		/// correctly reported all its possible moves. 
		/// </summary>
		[Fact]
		public void checkAllOfKingsStartingMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "f2, f4",
				 "f7, f5",
				 "e2, e4",
				 "e7, e5",
				 "d2, d4",
				 "d7, d5",
				 "c2, c4",
				 "c7, c5",
				 "g1, h3",
				 "g8, h6",
				 "b1, a3",
				 "b8, a6",
				 "f1, d3",
				 "f8, d6",
				 "c1, e3",
				 "c8, e6",
				 "d1, f3",
				 "d8, f6"
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("e1"));
			expected.Should().Contain(Move("e1, c1"))
				 .And.Contain(Move("e1, d1"))
				 .And.Contain(Move("e1, d2"))
				 .And.Contain(Move("e1, e2"))
				 .And.Contain(Move("e1, f2"))
				 .And.Contain(Move("e1, f1"))
				 .And.Contain(Move("e1, g1"))
				 .And.HaveCount(7, "the white king is in its starting position, " +
				 "and all pieces directly next to him are clear, and the " +
				 "knight on both side has been moved, should have 7 places to move");

			Apply(b, Move("e1, g1"));

			poss = b.GetPossibleMoves();
			expected = GetMovesAtPosition(poss, Pos("e8"));
			expected.Should().Contain(Move("e8, c8"))
				 .And.Contain(Move("e8, d8"))
				 .And.Contain(Move("e8, d7"))
				 .And.Contain(Move("e8, e7"))
				 .And.Contain(Move("e8, f7"))
				 .And.Contain(Move("e8, f8"))
				 .And.Contain(Move("e8, g8"))
				 .And.HaveCount(7, "the black king is in its starting position, " +
				 "and all pieces directly next to him are clear, and the " +
				 "knight on the both side has been moved, should have 7 places to move");
		}

		[Fact]
		public void checkFordoubleCheckMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "f7, f5",
				 "e4, f5",
				 "e7, e6",
				 "g2, g4",
				 "e6, f5",
				 "g4, f5",
				 "g7, g6",
				 "f5, g6",
				 "h7, g6",
				 "f2, f4",
				 "g6, g5",
				 "f4, g5",
				 "f8, d6",
				 "g5, g6",
				 "h8, h7",
				 "h2, h4",
				 "d6, e5",
				 "h4, h5",
				 "h7, e7",
				 "d2, d3",
				 "e5, g3"
			);

			var poss = b.GetPossibleMoves();
			var expected = GetMovesAtPosition(poss, Pos("e1"));
			expected.Should().Contain(Move("e1, d2"))
				 .And.HaveCount(1, "The King is in check by two pieces, "
									  + "and needs to be moved into the only available space ");
		}

		/// <summary>
		/// Check for impossible king-side castle by Player2
		/// </summary>
		[Fact]
		public void Player2KingBlockedCastleCheck() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "a7, a6",
				 "f1, c4",
				 "g8, f6",
				 "g1, f3",
				 "g7, g6",
				 "d2, d3",
				 "f8, g7",
				 "c1, d2",
				 "e7, e5",
				 "d2, b4"    //Puts Player2 King in check
			);

			var possMoves = b.GetPossibleMoves();
			var BlockedPlayer2KingCastle = GetMovesAtPosition(possMoves, Pos("e8"));
			BlockedPlayer2KingCastle.Should().HaveCount(0)
				 .And.NotContain(m => m.MoveType == ChessMoveType.CastleKingSide,
				 "Player1 Bishop blocking Player2 King-side castle, also Player2 King is checked");
		}

		/// <summary>
		/// Put Player2 King in position where castling is impossible and King is in check
		/// Undo where King is NOT in check and in position to castle. Check to see if castling and moves are valid
		/// </summary>
		[Fact]
		public void Player2UndoKingSideCastle() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "a7, a6",
				 "f1, c4",
				 "g8, f6",
				 "g1, f3",
				 "g7, g6",
				 "d2, d3",
				 "f8, g7",
				 "c1, d2",
				 "e7, e5",
				 "d2, b4"
			);

			var possMoves = b.GetPossibleMoves();
			var BlockedPlayer2KingCastle = GetMovesAtPosition(possMoves, Pos("e8"));
			BlockedPlayer2KingCastle.Should().NotContain(m => m.MoveType == ChessMoveType.CastleKingSide,
				 "Player1 Bishop blocking Player2 King-side castle");
			b.UndoLastMove();
			b.UndoLastMove();
			possMoves = b.GetPossibleMoves();
			BlockedPlayer2KingCastle = GetMovesAtPosition(possMoves, Pos("e8"));
			BlockedPlayer2KingCastle.Should().HaveCount(2)
				 .And.Contain(m => m.MoveType == ChessMoveType.CastleKingSide,
				 "Player1 Bishop is NOT blocking Player2 King-side castle");
		}

		[Fact]
		public void CheckingLongShortCastling() {
			//short castling
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("a1"), ChessPieceType.Rook, 1,
				 Pos("a8"), ChessPieceType.Rook, 2,
				 Pos("h1"), ChessPieceType.Rook, 1,
				 Pos("h8"), ChessPieceType.Rook, 2,
				 Pos("f7"), ChessPieceType.Pawn, 2

			);
			//White Mobe First
			Apply(b, Move("e1, g1"));
			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.Rook,
				 "Short Castling For Player 1");

			Apply(b, Move("e8, g8"));
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook,
				 "Short Castling For Player 2");

			//long castling
			ChessBoard c = CreateBoardFromPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a1"), ChessPieceType.Rook, 1,
				 Pos("a8"), ChessPieceType.Rook, 2,
				 Pos("h1"), ChessPieceType.Rook, 1,
				 Pos("h8"), ChessPieceType.Rook, 2,
				 Pos("d7"), ChessPieceType.Pawn, 2
			);
			Apply(c, "e1, c1");
			c.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Rook,
				 "Long Castling For Player 1");
			Apply(c, "e8, c8");
			c.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Rook,
				 "Long Castling For Player 2");

		}

		/// <summary>
		/// Check the black's king castle kingside. Check positions after castle is made.
		/// </summary>
		[Fact]
		public void BlackCastlingExample() {
			ChessBoard b = CreateBoardFromMoves(
				 "a2, a3",
				 "g8, h6",
				 "b2, b3",
				 "g7, g6",
				 "c2, c3",
				 "f8, g7",
				 "d2, d3"
			);
			//Check if castling on Black King is in possible moves
			var possMoves = b.GetPossibleMoves();
			var forKing = GetMovesAtPosition(possMoves, Pos("e8"));
			forKing.Should().HaveCount(2, "Black's king at e8 can castle kingside")
				 .And.BeEquivalentTo(Move("e8, f8"), Move("e8, g8"));

			//Check Rook's position after castling
			Apply(b, "e8, g8");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.King, "Black's King at position (6,0) after castling");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Rook, "Black's rook at position (5,0) after castling");
		}

		/*
		6. Attempt tp capture king
		 */
		[Fact]
		public void AttackKing() {
			ChessBoard board = CreateBoardFromPositions(
				Pos("g8"), ChessPieceType.Queen, 1,
				Pos("h8"), ChessPieceType.King, 2,
				Pos("h7"), ChessPieceType.Pawn, 2,
				Pos("g7"), ChessPieceType.Pawn, 2,
				Pos("f7"), ChessPieceType.Pawn, 2,
				Pos("a1"), ChessPieceType.King, 1
			);

			board.IsStalemate.Should().BeFalse("No Stalemate");
			var possMoves = board.GetPossibleMoves();
			var possMovesBlackKing = GetMovesAtPosition(possMoves, Pos("h8"));
			possMovesBlackKing.Should().HaveCount(0, "The black king cannot move out of check");
			var whiteQueenNoMoves = GetMovesAtPosition(possMoves, Pos("g8"));
			whiteQueenNoMoves.Should().Contain(Move("g8, h8"), "Queen should be able to capture king at g8");
			board.GetAttackedPositions(1).Should().Contain(Pos("h8"));
			Apply(board, Move(Pos("g8"), Pos("h8"), ChessMoveType.Normal));
			board.GetPieceAtPosition(Pos("h8")).Player.Should().Be(1, "White queen captured Black King");


		}
	}
}
