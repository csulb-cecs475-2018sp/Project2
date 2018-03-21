using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Cecs475.BoardGames.Chess.Model;
using Xunit;
using System.Linq;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Test {
	public class GameEndTests : ChessTest {
		/// <summary>
		/// White surrounds and places Black King in check;
		/// Test bench must validate only one possible move for Black
		/// </summary>
		[Fact]
		public void BlackCheckTest() {
			List<Tuple<BoardPosition, ChessPiece>> startingPositions = new List<Tuple<BoardPosition, ChessPiece>>();

			// White Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 4), new ChessPiece(ChessPieceType.King, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(6, 4), new ChessPiece(ChessPieceType.Queen, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(7, 5), new ChessPiece(ChessPieceType.Rook, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 0), new ChessPiece(ChessPieceType.Rook, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 1), new ChessPiece(ChessPieceType.Bishop, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(2, 2), new ChessPiece(ChessPieceType.Bishop, 1)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(4, 2), new ChessPiece(ChessPieceType.Knight, 1)));

			// Black Positions
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 4), new ChessPiece(ChessPieceType.King, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(0, 2), new ChessPiece(ChessPieceType.Rook, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(3, 4), new ChessPiece(ChessPieceType.Rook, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(1, 6), new ChessPiece(ChessPieceType.Bishop, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(1, 3), new ChessPiece(ChessPieceType.Knight, 2)));
			startingPositions.Add(new Tuple<BoardPosition, ChessPiece>(new BoardPosition(1, 4), new ChessPiece(ChessPieceType.Pawn, 2)));

			ChessBoard b = new ChessBoard(startingPositions);

			// Move Knight to to place Black King in Check Position
			Apply(b, Move("c4, d6"));


			// Black should be in Check
			b.IsCheck.Should().BeTrue("Black's King (e4) is in Check from Knight (d6)");

			var possMoves = b.GetPossibleMoves();

			possMoves.Should().HaveCount(1, "Only one possible legal move to get out of check: Pawn takes Knight at d6")
					  .And.Contain(new ChessMove(Pos("e7"), Pos("d6"), ChessMoveType.Normal), "Move must be Pawn at E8 taking Knight at D6");
		}

		[Fact]
		public void WhiteKingCheckmate() {
			ChessBoard board = CreateBoardFromMoves(
			  "e2, e4",
			  "e7, e5",
			  "d1, h5",
			  "d8, e7",
			  "h5, e5",
			  "e7, e5",
			  "c2, c4",
			  "d7, d5",
			  "d2, d4",
			  "e5, e4",
			  "e1, d1",
			  "e4, g4",
			  "d1, d2",
			  "f8, b4",
			  "d2, d3",
			  "g4, f4",
			  "b1, d2",
			  "g8, h6",
			  "f1, e2",
			  "c8, f5",
			  "d2, e4",
			  "f5, e4"
			);
			board.IsCheckmate.Should().BeTrue("the king has no escape");
		}

		[Fact]
		public void FiftyMovesStalemate() {
			ChessBoard b = new ChessBoard();
			for (int i = 0; i < 25; i++) {
				Apply(b, Move("b1, c3"));
				Apply(b, Move("g8, h6"));
				Apply(b, Move("c3, b1"));
				Apply(b, Move("h6, g8"));
			}
			b.IsDraw.Should().Be(true, "if neither player wins in 50 moves, the game is a draw");
		}

		//Test 2
		//Testing if the checkmate works
		[Fact]
		public void FourMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "e7, e5",
				 "f1, c4",
				 "b8, c6",
				 "d1, h5",
				 "g8, f6",
				 "h5, f7");
			b.IsCheckmate.Should().Be(true, "The king should be on checkmate");
			var moves = b.GetPossibleMoves();
			moves.Should().BeEmpty();
		}

		//Test 3
		//Testing if undo removes checkmate
		[Fact]
		public void UndoCheckMate() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "e7, e5",
				 "f1, c4",
				 "b8, c6",
				 "d1, h5",
				 "g8, f6",
				 "h5, f7");
			b.IsCheckmate.Should().Be(true, "The king should be on checkmate");
			b.UndoLastMove();
			b.IsCheckmate.Should().Be(false, "Undoing a checkmate should remove the checkmate");
		}

		//Stalemate test
		[Fact]
		public void Stalemate() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a8"), ChessPieceType.King, 2,
				 Pos("h1"), ChessPieceType.King, 1,
				 Pos("g8"), ChessPieceType.Rook, 2,
				 Pos("a2"), ChessPieceType.Rook, 2
			);
			b.IsStalemate.Should().BeTrue("There shouldn't be any possible moves for the king!");
		}

		/// <summary>
		/// Test the King in check and the possible moves that result from that
		/// </summary>
		[Fact]
		public void KingInCheck2() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e3"), ChessPieceType.Bishop, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("h8"), ChessPieceType.King, 2
			);

			b.IsCheck.Should().BeFalse("Black is not in danger");
			Apply(b, Move("e3, d4"));
			b.IsCheck.Should().BeTrue("Black is in check by Bishop at d4");

			var possMoves = b.GetPossibleMoves();
			possMoves.Count().Should().Be(2, "There should be 2 moves from h8 King");
			possMoves.Should().Contain(Move("h8, h7"), "h8 to h7 should be a valid move")
			.And.Contain(Move("h8, g8"), "h8 to g8 should be a valid move")
			.And.NotContain(Move("h8, g7"), "h8 to g7 should not be a valid move because of a check from Bishop");

			b.UndoLastMove();
			b.IsCheck.Should().BeFalse("Black is not in danger because of undo");

		}

		[Fact]
		public void BlackKingIsCheck() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e4",
				 "e7, e5",
				 "f1, e2",
				 "f7, f5",
				 "e2, h5"
			);

			b.GetPieceAtPosition(Pos("h5")).PieceType.Should().Be(ChessPieceType.Bishop, "Bishop should be at position h5");

			b.IsCheck.Should().BeTrue("the king is threatened by rook but has an escape");

			var possMoves = b.GetPossibleMoves();
			var oneMove = GetMovesAtPosition(possMoves, Pos("e8"));
			oneMove.Should().HaveCount(1, "King has one escape move");
		}

		/// <summary>
		/// Four move checkmate
		/// </summary>
		[Fact]
		public void fourMove_Checkmate() {
			ChessBoard b = CreateBoardFromMoves(
				"e2, e4",   // white pawn up 2
				"e7, e5",   // black pawn down 2
				"f1, c4", // bishop out
				"b8, c6", // black knight out
				"d1, h5", // white queen out
				"g8, f6" // black knight out
			);

			Apply(b, "h5, f7");
			var moves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(moves, Pos("e8"));
			kingMoves.Should().HaveCount(0, "black has no moves");
			b.CurrentAdvantage.Should().Be(Advantage(1, 1), "Black lost a single pawn of 1 value");
			b.IsCheckmate.Should().BeTrue("King threatend by queen king can not capture queen because of bishop at c4");
		}

		/// <summary>
		/// Rook captures pawn at e6 resulting in a stale mate
		/// </summary>
		[Fact]
		public void PawnCapture_IntoStalemate() {

			ChessBoard b = CreateBoardFromPositions(
				Pos("e1"), ChessPieceType.King, 1,
				Pos("f8"), ChessPieceType.King, 2,
				Pos("h7"), ChessPieceType.Queen, 1,
				Pos("a6"), ChessPieceType.Rook, 1,
				Pos("e6"), ChessPieceType.Pawn, 2
			);
			b.CurrentAdvantage.Should().Be(Advantage(1, 13)); // Advantage before capture
																			  //Console.Write(ConsoleView.BoardToString(b)); // Draw out board
			Apply(b, "a6, e6");
			b.CurrentAdvantage.Should().Be(Advantage(1, 14), "Player 2 pawn was captured"); // Advantage after capturing pawn

			b.GetPieceAtPosition(Pos("e6")).Player.Should().Be(1, "Player 1's rook captured Player 2's pawn");

			var moves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(moves, Pos("f8"));
			kingMoves.Should().HaveCount(0, "Player 2 has no moves");
			b.IsStalemate.Should().BeTrue("Player 2's king has no move that will allow him to not be in check");

		}

		/// <summary>
		/// Black castles queen side into checkmate
		/// </summary>
		[Fact]
		public void castling_IntoCheckmate() {

			ChessBoard b = CreateBoardFromPositions(
				Pos("d4"), ChessPieceType.King, 1,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("e7"), ChessPieceType.Queen, 2,
				Pos("a8"), ChessPieceType.Rook, 2,
				Pos("c1"), ChessPieceType.Rook, 2
			);

			//Console.Write(ConsoleView.BoardToString(b)); // Draw out board

			var moves = b.GetPossibleMoves();
			var kingMoves = GetMovesAtPosition(moves, Pos("d4"));
			kingMoves.Should().HaveCount(2, "black has no moves")
				.And.BeEquivalentTo(Move("d4, d5"), Move("d4, d3"));

			Apply(b, "d4, d5"); // Move king so it is blacks turn

			b.CurrentPlayer.Should().Be(2, "Player 2's turn");

			// Check black can castle queen side
			moves = b.GetPossibleMoves();
			kingMoves = GetMovesAtPosition(moves, Pos("e8"));
			kingMoves.Should().HaveCount(5, "Black should have 4 regular moves and be able to castle queen side")
				.And.BeEquivalentTo(Move("e8, d7"), Move("e8, d8"), Move("e8, f7"), Move("e8, f8"), Move("e8, c8"));

			Apply(b, "e8, c8"); // black king castles queen side

			kingMoves = GetMovesAtPosition(moves, Pos("d5"));
			kingMoves.Should().HaveCount(0, "White is in check and has no moves");
			b.IsCheckmate.Should().BeTrue("White king in checkmate");

		}

		/// <summary>
		/// Use black king to check all possible moves available while in check
		/// </summary>
		[Fact]
		public void KingInCheck_PossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(
				 Move("d2, d4"),
				 Move("g8, h6"),
				 Move("d1, d3"),
				 Move("g7, g6"),
				 Move("a2, a3"),
				 Move("f8, g7"),
				 Move("b2, b3"),
				 Move("g7, d4"),
				 Move("c2, c3"),
				 Move("f7, f6"),
				 Move("e2, e3"),
				 Move("h8, g8"),
				 Move("d3, g6")
				 );

			// check if black king is currently in check
			b.CurrentPlayer.Should().Be(2, "player is black");
			b.IsCheck.Should().Be(true, "because king is in check");

			var threeMovesExpected = b.GetPossibleMoves();
			threeMovesExpected.Should().Contain(Move("e8, f8"))
				 .And.Contain(Move("h6, f7"))
				 .And.Contain(Move("g8, g6"))
				 .And.Contain(Move("h7, g6"))
				 .And.HaveCount(4, "there are only four moves where king can avoid check");
		}

		/// <summary>
		/// The test checks if the king is in checkmate or not before and after the promotion. 
		/// Test : - UndoLastMove
		/// Player: - White, Black
		/// Result: - After promoting the white pawn to a queen, the black king is at checkmate, but after undoing the move the
		/// the king is no more at checkmate.
		/// </summary>
		[Fact]
		private void CheckMateAfterPromotion() {
			ChessBoard board = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a5"), ChessPieceType.Bishop, 1,
				 Pos("a6"), ChessPieceType.Queen, 1,
				 Pos("a8"), ChessPieceType.Rook, 1,
				 Pos("g7"), ChessPieceType.Pawn, 1,
				 Pos("d7"), ChessPieceType.King, 2);

			board.IsCheck.Should().BeFalse("The black king can move from d7 to either c7 or e7");

			//White rook move
			Apply(board, "a8,a7");
			board.IsCheck.Should().BeTrue("The black king is being checked by white rook which just moved to a7");
			board.IsCheckmate.Should().BeFalse("The black king can still move to e8");

			//Black king move
			Apply(board, "d7,e8");
			board.IsCheck.Should().BeFalse("The black king is not being checked by any white piece");

			//White pawn move to promote
			Apply(board, Move("(g7,g8,Queen)"));
			board.IsCheckmate.Should().BeTrue("The black king is checked by promoted white queen and cannot move anywhere");

			//check player one advantage
			// board.CurrentAdvantage.Should().Be(Advantage(1,), "The black only has a king whereas white has king, bishop, queen," +
			//    "rook, and the promoted queen");

			board.CurrentPlayer.Should().Be(2, "It is player 2's turn after the white promoted to pawn");

			board.UndoLastMove();

			board.CurrentPlayer.Should().Be(1, "It is player 1's turn after undoing the move");

			board.IsCheckmate.Should().BeFalse("The black king is not at check anymore because the promoted queen is not there");
		}

		/// <summary>
		/// The test checks if castling is possible when the new position of the king will be in check. 
		/// Test : - UndoLastMove
		/// Player: - White
		/// Result: - In the given layout of the board, castling cannot be perfomred because the white king at new position, 
		/// c1 would be in check, but if we moved the knight away from b3 by undoing the move, castling should be possible 
		/// </summary>
		[Fact]
		public void CastlingLeadingToCheck() {
			ChessBoard board = CreateBoardFromMoves(
				 "b2, b4",
				 "b8, c6",
				 "c1, a3",
				 "c6, d4",
				 "b1, c3",
				 "f7, f6",
				 "e2, e3",
				 "f6, f5",
				 "d1, e2");

			//Move the black knight to put c1 as a position that can be captured
			Apply(board, "d4, b3");

			var possibleMoves = board.GetPossibleMoves();
			var castlingKingMoves = GetMovesAtPosition(possibleMoves, Pos("e1"));
			castlingKingMoves.Should().HaveCount(1, "The king cannot castle because the black knight would result in check")
			  .And.NotContain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide));

			board.UndoLastMove();

			board.CurrentPlayer.Should().Be(2, "It is player 2's turn after undoing the move");

			//move the black knight to a different position
			Apply(board, "d4,b5");

			possibleMoves = board.GetPossibleMoves();
			castlingKingMoves = GetMovesAtPosition(possibleMoves, Pos("e1"));
			castlingKingMoves.Should().HaveCount(2, "The king can castle as it would not lead to check")
				 .And.Contain(Move(Pos("e1"), Pos("c1"), ChessMoveType.CastleQueenSide));

		}

		[Fact]
		public void Player1KingInCheck() {
			ChessBoard b = CreateBoardFromMoves(
				 "g2, g4",
				 "d7, d5",
				 "e2, e3",
				 "d8, d6",
				 "d2, d4",
				 "d6, b4"
			);

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(6, "King can move or check can be blocked");
		}

		/// <summary>
		/// Testing possible moves from placing a king in check
		/// </summary>
		[Fact]
		public void KingInCheck3() {
			ChessBoard b = CreateBoardFromMoves(
				"d2, d4",
				"d7, d5",
				"e2, e4",
				"e7, e6",
				"h2, h4",
				"f8, b4"
			);
			b.IsCheck.Should().BeTrue("the king is in check");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(6, "the king can move out of check")
				.And.Contain(Move("e1, e2"))
				.And.Contain(Move("d1, d2"))
				.And.Contain(Move("c1, d2"))
				.And.Contain(Move("b1, d2"))
				.And.Contain(Move("b1, c3"))
				.And.Contain(Move("c2, c3"));
			b.UndoLastMove();
			b.IsCheck.Should().BeFalse("the king is no longer in check as the last move was undone");
		}

		/// <summary>
		/// Check king check with a pawn
		/// </summary>
		[Fact]
		public void checkKingCheck() {
			ChessBoard b = new ChessBoard();

			Apply(b, "b2, b4"); // Move p1's pawn
			Apply(b, "c7, c5"); // Move p2's pawn
			Apply(b, "b4, c5"); // Take p2's pawn with p1's pawn

			Apply(b, "d7, d5"); // Move p2's pawn to make a en passant situation
			Apply(b, "c5, d6");   // Move p1's pawn
			Apply(b, "d8, c7"); // Move p2's queen
			Apply(b, "d6, d7"); // Move p1's pawn to create a check situation
			b.IsCheck.Should().BeTrue("the p2's king is threatened");
			GetMovesAtPosition(b.GetPossibleMoves(), Pos("e7")).Should().HaveCount(0, "there is 0 possible moves for p2's pawn at this stage of game because the king is in check situation");
			GetMovesAtPosition(b.GetPossibleMoves(), Pos("e8")).Should().HaveCount(2, "there is 2 possible moves for p2's king at this stage of game");
		}

		/// <summary>
		/// Tests to see if the DrawCounter logic works based on the rules of a chess draw.
		/// If the DrawCounter property reaches 100, then the game should be finished due to a draw.
		/// </summary>
		[Fact]
		public void DrawCounterFinish() {
			ChessBoard b = new ChessBoard();

			Apply(b, Move("a2, a3"), Move("a7, a6"));
			for (int i = 0; i < 25; i++) {
				Apply(b, Move("a1, a2"), Move("a8, a7"), Move("a2, a1"), Move("a7, a8"));
			}
			b.DrawCounter.Should().Be(100, ", but it counted " + b.DrawCounter);
			b.IsDraw.Should().Be(true, "there were no advancing player moves for 50 turns, game is a draw");
			b.IsFinished.Should().Be(true, "the game is finished by a draw");
		}

		/// <summary>
		/// Puts black king in check and only allows three possible
		/// moves for player 2 to get out of check.
		/// </summary>
		[Fact]
		public void PossibleMovesWhileInCheck() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("g5"), ChessPieceType.Bishop, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("c7"), ChessPieceType.Pawn, 2,
				 Pos("d2"), ChessPieceType.Pawn, 1,
				 Pos("b5"), ChessPieceType.Queen, 1
			 );

			Apply(b, Move("d2, d3"));
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(3, "king has two options for movement and pawn has one option for movement").And.BeEquivalentTo(Move("c7, c6"),
				 Move("e8, f7"), Move("e8, f8"));
		}

		/// <summary>
		/// The purpose of this test is to test the available moves of the black king
		/// when only 1 move is possible, when he is surrounded, on check and checkmate
		/// </summary>
		[Fact]
		public void BlackKingMovesOnCheckPlusCheckMate() {
			ChessBoard b = CreateBoardFromPositions(
				 Pos("a6"), ChessPieceType.King, 1,
				 Pos("e3"), ChessPieceType.Queen, 1,
				 Pos("b8"), ChessPieceType.Bishop, 1,
				 Pos("a4"), ChessPieceType.Rook, 1,
				 Pos("h7"), ChessPieceType.Rook, 1,
				 Pos("d5"), ChessPieceType.King, 2
			);
			Apply(b, "a6, a7");
			var possMoves = b.GetPossibleMoves();
			var BlackKingMoves = GetMovesAtPosition(possMoves, Pos("d5"));
			BlackKingMoves.Should().HaveCount(1, "King should be attacked from all around except from c6")
				 .And.Contain(Move("d5, c6"));
			b.IsCheck.Should().BeFalse("the king is not in check");
			b.UndoLastMove();
			Apply(b, "a6, b6");
			possMoves = b.GetPossibleMoves();
			BlackKingMoves = GetMovesAtPosition(possMoves, Pos("d5"));
			BlackKingMoves.Should().BeEmpty("King should be attacked from all around with no moves available");
			b.IsCheck.Should().BeFalse("The king is not in check");
			b.UndoLastMove();
			Apply(b, "e3, e4");
			b.IsCheck.Should().BeTrue("The king is in check by the queen in e4");
			Apply(b, "d5, c5");
			b.IsCheck.Should().BeFalse("The king is not in check");
			Apply(b, "a4, a5");
			b.IsCheck.Should().BeFalse("The king is not in check but in checkmate");
			b.IsCheckmate.Should().BeTrue("The king is in checkmate");
		}

		///<summary>
		/// Determines the presence of a check from P1
		///</summary>
		[Fact]
		public void ValidateCheckFromWhite() {
			ChessBoard cb = CreateBoardFromPositions(
				 Pos(1, 6), ChessPieceType.Rook, 2,
				 Pos(2, 2), ChessPieceType.King, 2, //black king to be checked
				 Pos(3, 4), ChessPieceType.Bishop, 2,
				 Pos(4, 1), ChessPieceType.Bishop, 1,
				 Pos(6, 2), ChessPieceType.Rook, 1, //white rook to check king
				 Pos(6, 5), ChessPieceType.King, 1);

			//move white bishop and switch to P2
			Apply(cb, "b4, a5");
			var possMoves = cb.GetPossibleMoves();
			cb.IsCheck.Should().BeTrue("Black king is in check from white rook at c2");
		}

		/// <summary>
		/// Check a immediate checkmate, not check.
		/// </summary>
		[Fact]
		public void BlackCheckMate() {
			ChessBoard b = CreateBoardFromMoves(
				 "e2, e3",
				 "g7, g5",
				 "b2, b3",
				 "f7, f5",
				 "d1, h5"
			);
			//Check if Black's King is in checkmate
			var possMoves = b.GetPossibleMoves();
			b.IsCheckmate.Should().BeTrue("black's king is in checkmate from queen at h5, & has no way to get out of check");
			b.IsCheck.Should().BeFalse("Checkmate and Check can't be both true");
		}

		[Fact]
		public void BlackKingCheck() {
			ChessBoard b = CreateBoardFromPositions(
							Pos("d7"), ChessPieceType.Pawn, 2,
							Pos("f3"), ChessPieceType.Pawn, 1,
							Pos("e8"), ChessPieceType.King, 2,
							Pos("e1"), ChessPieceType.King, 1,
							Pos("h6"), ChessPieceType.Rook, 2,
							Pos("d3"), ChessPieceType.Rook, 1,
							Pos("c6"), ChessPieceType.Queen, 2,
							Pos("h4"), ChessPieceType.Queen, 1,
							Pos("c4"), ChessPieceType.Knight, 2,
							Pos("g6"), ChessPieceType.Knight, 1
					  );
			// White moves rook for check
			Apply(b, "d3, e3");

			b.IsCheck.Should().BeTrue("black's king is in check from rook at e3");

			// Checks Moves for Black's King
			var possMoves = b.GetPossibleMoves();
			possMoves.Count().Should().Be(5, "There should be 5 possible moves to cancel the checking of black's king");

			// Validating GetPossivleMoves() when a check occurs
			possMoves.Should().Contain(Move("e8, f7"), "the king can move one space")
			.And.Contain(Move("c6, e4"), "queen moves to block white rook, cancelling the check")
			.And.Contain(Move("c4, e3"), "knight captures white rook")
			.And.Contain(Move("c4, e5"), "knight moves to block white rook, cancelling the check")
			.And.Contain(Move("c6, e6"), "queen captures white knight");
		}

		/*
		4. Check for king to be in check 
		*/
		[Fact]
		public void KingInCheck() {
			ChessBoard board = CreateBoardFromMoves(
				"g2, g3",
				"e7, e5",
				"f1, h3",
				"d7, d6",
				"h3, g4");//bishop at c8 is underattack by bishop at g4 by player 1

			//Check attacked position for bishop at c8 
			var attackedPositions = board.GetAttackedPositions(1);
			attackedPositions.Should().Contain(Pos("c8"), "bishop at g4 can capture by enemy bishop at c8");

			Apply(board, "f7, f5");//block attack with pawn
			attackedPositions = board.GetAttackedPositions(1);
			attackedPositions.Should().NotContain(Pos("c8"), "bishop at g4 cannot be captured by enemy bishop at c8");

			Apply(board, "g4, h5");//king now in check
			board.IsCheck.Should().BeTrue("the king at e8 is in check");

			var possMoves = board.GetPossibleMoves();
			var expectedMovesForKing = GetMovesAtPosition(possMoves, Pos("e8"));
			expectedMovesForKing.Should().HaveCount(2, "king at e8 should have 2 moves to e7 or d7, not f7")
				.And.Contain(Move("e8,e7"));
			Apply(board, "e8, e7");
			board.IsCheck.Should().BeFalse("the king at e8 is not in check");

			board.UndoLastMove();
			board.IsCheck.Should().BeTrue("the king at e8 is back in check after undo");
		}
	}
}
