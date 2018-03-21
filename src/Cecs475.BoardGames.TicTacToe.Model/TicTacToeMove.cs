using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.TicTacToe.Model {
	public class TicTacToeMove : IGameMove, IEquatable<TicTacToeMove> {
		public BoardPosition Position { get; private set; }

		public int Player { get; set; }

		public TicTacToeMove(BoardPosition position) {
			Position = position;
		}

		public bool Equals(IGameMove other) {
			return Equals(other as TicTacToeMove);
		}

		public bool Equals(TicTacToeMove other) {
			return Position.Equals(other.Position);
		}
	}
}
