using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.TicTacToe.WpfView {
	/// <summary>
	/// Interaction logic for TicTacToeView.xaml
	/// </summary>
	public partial class TicTacToeView : UserControl, IWpfGameView {
		public static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.Red);
		public static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.Green);

		public Control ViewControl => this;

		public IGameViewModel ViewModel => TicTacToeViewModel;

		private TicTacToeViewModel TicTacToeViewModel => FindResource("vm") as TicTacToeViewModel;

		public TicTacToeView() {
			InitializeComponent();
		}

		private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
			Border b = sender as Border;
			var square = b.DataContext as TicTacToeSquare;
			var vm = FindResource("vm") as TicTacToeViewModel;
			if (vm.PossibleMoves.Contains(square.Position)) {
				vm.ApplyMove(square.Position);
				b.Background = GREEN_BRUSH;
			}
		}

		private void Border_MouseEnter(object sender, MouseEventArgs e) {
			Border b = sender as Border;
			var square = b.DataContext as TicTacToeSquare;
			var vm = FindResource("vm") as TicTacToeViewModel;
			if (vm.PossibleMoves.Contains(square.Position)) {
				b.Background = RED_BRUSH;
			}
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e) {
			Border b = sender as Border;
			b.Background = GREEN_BRUSH;
		}
	}
}
