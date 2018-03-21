using System.Windows.Data;
using Cecs475.BoardGames.WpfView;

namespace Cecs475.BoardGames.TicTacToe.WpfView {
	public class TicTacToeGameFactory : IWpfGameFactory {
		public string GameName {
			get {
				return "Tic Tac Toe";
			}
		}

		public IValueConverter CreateBoardAdvantageConverter() {
			return new TicTacToeAdvantageConverter();
		}

		public IValueConverter CreateCurrentPlayerConverter() {
			return new TicTacToeCurrentPlayerConverter();
		}

		public IWpfGameView CreateGameView() {
			return new TicTacToeView();
		}
	}
}
