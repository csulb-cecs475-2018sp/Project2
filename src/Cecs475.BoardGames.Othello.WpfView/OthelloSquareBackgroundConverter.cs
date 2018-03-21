using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Othello.WpfView {
	class OthelloSquareBackgroundConverter : IMultiValueConverter {
		private static SolidColorBrush HIGHLIGHT_BRUSH = Brushes.Red;
		private static SolidColorBrush CORNER_BRUSH = Brushes.Green;
		private static SolidColorBrush SIDE_BRUSH = Brushes.LightGreen;
		private static SolidColorBrush DANGER_BRUSH = Brushes.PaleVioletRed;
		private static SolidColorBrush DEFAULT_BRUSH = Brushes.LightBlue;

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			// This converter will receive two properties: the Position of the square, and whether it
			// is being hovered.
			BoardPosition pos = (BoardPosition)values[0];
			bool isHighlighted = (bool)values[1];

			// Hovered squares have a specific color.
			if (isHighlighted) {
				return HIGHLIGHT_BRUSH;
			}
			// Corner squares are very good, and drawn green.
			if ((pos.Row == 0 || pos.Row == 7) && (pos.Col == 0 || pos.Col == 7)) {
				return CORNER_BRUSH;
			}
			// Squares next to corners are very bad, and drawn pale red.
			if ((pos.Row == 0 || pos.Row == 1 || pos.Row == 6 || pos.Row == 7)
				&& (pos.Col == 0 || pos.Col == 1 || pos.Col == 6 || pos.Col == 7)) {
				return DANGER_BRUSH;
			}
			// Squares along the edge are good, and drawn light green.
			if (pos.Row == 0 || pos.Row == 7 || pos.Col == 0 || pos.Col == 7) {
				return SIDE_BRUSH;
			}
			// Inner squares are drawn light blue.
			return DEFAULT_BRUSH;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}