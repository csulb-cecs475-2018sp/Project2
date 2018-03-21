using Cecs475.BoardGames;
using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.TicTacToe.Model {
	/// <summary>
	/// Represents the board state for a game of tic tac toe.
	/// </summary>
	public class TicTacToeBoard : IGameBoard {
		private int mPlayer;
		private sbyte[,] mBoard = new sbyte[3, 3];

		private List<TicTacToeMove> mMoveHistory = new List<TicTacToeMove>();

		public TicTacToeBoard() {
			mPlayer = 1;
		}

		public int CurrentPlayer {
			get {
				return mPlayer == -1 ? 2 : 1;
			}
		}

		/// <summary>
		/// Value is either 0, or 1 if Player 1 has won, or -1 if Player 2 has won.
		/// </summary>
		private int mValue;

		public IReadOnlyList<TicTacToeMove> MoveHistory => mMoveHistory;

		public bool IsFinished { get; private set; }

		public GameAdvantage CurrentAdvantage => new GameAdvantage(mValue, mValue == 0 ? 0 : mValue > 0 ? 1 : 2);

		IReadOnlyList<IGameMove> IGameBoard.MoveHistory => throw new NotImplementedException();

		public void ApplyMove(TicTacToeMove m) {
			SetPosition(m.Position, CurrentPlayer);
			mMoveHistory.Add(m);
			mPlayer = -mPlayer;
			IsFinished = GameIsOver();
		}

		/// <summary>
		/// True if one player has 3 squares in a row. Sets Value 1 if Player1 has won, 
		/// or -1 if Player2 has won.
		/// </summary>
		/// <returns></returns>
		private bool GameIsOver() {
			mValue = 0;
			for (int r = 0; r < 3; r++) {
				if (mBoard[r, 0] == mBoard[r, 1] && mBoard[r, 0] == mBoard[r, 2] && mBoard[r, 0] != 0) {
					mValue = mBoard[r, 0];
				}
				if (mBoard[0, r] == mBoard[1, r] && mBoard[0, r] == mBoard[2, r] && mBoard[0, r] != 0) {
					mValue = mBoard[0, r];
				}
			}
			if (mBoard[0, 0] == mBoard[1, 1] && mBoard[0, 0] == mBoard[2, 2] && mBoard[0, 0] != 0) {
				mValue = mBoard[0, 0];
			}
			if (mBoard[0, 2] == mBoard[1, 1] && mBoard[0, 2] == mBoard[2, 0] && mBoard[0, 2] != 0) {
				mValue = mBoard[0, 2];
			}
			return mValue != 0;
		}

		private void SetPosition(BoardPosition position, int player) {
			mBoard[position.Row, position.Col] = (sbyte)(player == 2 ? -1 : player);
		}

		public IEnumerable<TicTacToeMove> GetPossibleMoves() {
			if (IsFinished) {
				return new TicTacToeMove[0];
			}
			return
				BoardPosition.GetRectangularPositions(3, 3)
				.Where(pos => GetPieceAtPosition(pos) == 0)
				.Select(pos => new TicTacToeMove(pos));
		}

		public int GetPieceAtPosition(BoardPosition pos) {
			int player = mBoard[pos.Row, pos.Col];
			return player == -1 ? 2 : player;
		}

		public void UndoLastMove() {
			TicTacToeMove m = MoveHistory.Last() as TicTacToeMove;
			SetPosition(m.Position, 0);
			mMoveHistory.RemoveAt(MoveHistory.Count - 1);
			mValue = 0;
			IsFinished = false;
		}

		IEnumerable<IGameMove> IGameBoard.GetPossibleMoves() {
			return GetPossibleMoves();
		}

		public void ApplyMove(IGameMove move) {
			ApplyMove(move as TicTacToeMove);
		}
	}
}
