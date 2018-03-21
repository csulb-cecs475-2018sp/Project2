using System;
using System.Collections.Generic;
using System.Text;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.Othello.Model;
using Cecs475.BoardGames.View;

namespace Cecs475.BoardGames.Othello.WpfView {
	public class OthelloConsoleView : IConsoleView {
		private static char[] LABELS = { '.', 'B', 'W' };

		public string BoardToString(OthelloBoard board) {
			StringBuilder str = new StringBuilder();
			str.AppendLine("- 0 1 2 3 4 5 6 7");
			for (int i = 0; i < OthelloBoard.BOARD_SIZE; i++) {
				str.Append(i);
				str.Append(" ");
				for (int j = 0; j < OthelloBoard.BOARD_SIZE; j++) {
					int space = board.GetPlayerAtPosition(new BoardPosition(i, j));
					str.Append(LABELS[space]);
					str.Append(" ");
				}
				str.AppendLine();
			}
			return str.ToString();
		}

		public string MoveToString(OthelloMove move) {
			return "(" + move.Position.Row + ", " + move.Position.Col + ")";
		}

		public OthelloMove ParseMove(string moveText) {
			string[] split = moveText.Trim(new char[] { '(', ')' }).Split(',');
			return new OthelloMove(
				new BoardPosition(Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
		}

		public string PlayerToString(int player) {
			return player == 1 ? "Black" : "White";
		}

		string IConsoleView.BoardToString(IGameBoard board) {
			return BoardToString(board as OthelloBoard);
		}

		string IConsoleView.MoveToString(IGameMove move) {
			return MoveToString(move as OthelloMove);
		}

		IGameMove IConsoleView.ParseMove(string moveText) {
			return ParseMove(moveText);
		}
	}
}
