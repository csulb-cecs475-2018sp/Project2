using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cecs475.BoardGames.TicTacToe.WpfView {
	class TicTacToeAdvantageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var v = (GameAdvantage)value;
			if (v.Player == 0)
				return "Tie game";
			if (v.Player > 0)
				return "X wins!";
			return "O wins!";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
