using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cecs475.BoardGames.Othello.WpfView {
	/// <summary>
	/// Represents an Othello game that can be played in a WPF app.
	/// </summary>
	public class OthelloGameFactory : IWpfGameFactory {
		public string GameName {
			get {
				return "Othello";
			}
		}

		public IValueConverter CreateBoardAdvantageConverter() {
			return new OthelloAdvantageConverter();
		}

		public IValueConverter CreateCurrentPlayerConverter() {
			return new OthelloCurrentPlayerConverter();
		}

		public IWpfGameView CreateGameView() {
			return new OthelloView();
		}
	}
}
