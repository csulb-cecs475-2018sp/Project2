using Cecs475.BoardGames;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.TicTacToe.Model;
using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.TicTacToe.WpfView {
	public class TicTacToeSquare : INotifyPropertyChanged {
		private int mPlayer;
		public int Player {
			get { return mPlayer; }
			set {
				if (value != mPlayer) {
					mPlayer = value;
					OnPropertyChanged(nameof(Player));
				}
			}
		}

		public BoardPosition Position {
			get; set;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public class TicTacToeViewModel : IGameViewModel, INotifyPropertyChanged {
		private TicTacToeBoard mBoard;
		private ObservableCollection<TicTacToeSquare> mSquares;

		public event EventHandler GameFinished;
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public TicTacToeViewModel() {
			mBoard = new TicTacToeBoard();
			mSquares = new ObservableCollection<TicTacToeSquare>(
				BoardPosition.GetRectangularPositions(3, 3)
				.Select(pos => new TicTacToeSquare() {
					Position = pos,
					Player = mBoard.GetPieceAtPosition(pos)
				})
			);

			PossibleMoves = new HashSet<BoardPosition>(
				mBoard.GetPossibleMoves().Select(m => m.Position)
			);
		}

		public void ApplyMove(BoardPosition position) {
			var possMoves = mBoard.GetPossibleMoves() as IEnumerable<TicTacToeMove>;
			foreach (var move in possMoves) {
				if (move.Position.Equals(position)) {
					mBoard.ApplyMove(move);
					break;
				}
			}

			RebindState();
			if (mBoard.IsFinished)
				GameFinished?.Invoke(this, new EventArgs());
		}

		private void RebindState() {
			PossibleMoves = new HashSet<BoardPosition>(mBoard.GetPossibleMoves().Select(m => m.Position));
			foreach (var square in mSquares) {
				square.Player = mBoard.GetPieceAtPosition(square.Position);
			}
			OnPropertyChanged(nameof(BoardAdvantage));
			OnPropertyChanged(nameof(CurrentPlayer));
		}

		public void UndoMove() {
			if (CanUndo) {
				mBoard.UndoLastMove();
				RebindState();
			}
		}

		public ObservableCollection<TicTacToeSquare> Squares {
			get { return mSquares; }
		}

		public HashSet<BoardPosition> PossibleMoves {
			get; private set;
		}

		public GameAdvantage BoardAdvantage => mBoard.CurrentAdvantage;

		public int CurrentPlayer => mBoard.CurrentPlayer;

		public bool CanUndo => mBoard.MoveHistory.Any();
	}

	/// <summary>
	/// Converts from an integer player number to an Ellipse representing that player's token.
	/// </summary>
	public class TicTacToeSquarePlayerConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			int player = (int)value;
			if (player == 0) {
				return null;
			}
			if (player == 2) {
				return new Ellipse() {
					Stroke = Brushes.Black,
					StrokeThickness = 5,
					Fill = null
				};
			}
			Canvas c = new Canvas();
			Line l1 = new Line() {
				Stroke = Brushes.Black,
				StrokeThickness = 5
			};
			l1.SetBinding(Line.X2Property,
				new Binding("ActualWidth") {
					Mode = BindingMode.OneWay,
					RelativeSource = new RelativeSource(
						RelativeSourceMode.FindAncestor, typeof(Border), 1)
				}
			);
			l1.SetBinding(Line.Y2Property,
				new Binding("ActualHeight") {
					Mode = BindingMode.OneWay,
					RelativeSource = new RelativeSource(
						RelativeSourceMode.FindAncestor, typeof(Border), 1)
				}
			);
			c.Children.Add(l1);

			Line l2 = new Line() {
				Stroke = Brushes.Black,
				StrokeThickness = 5
			};
			l2.SetBinding(Line.X1Property,
				new Binding("ActualWidth") {
					Mode = BindingMode.OneWay,
					RelativeSource = new RelativeSource(
						RelativeSourceMode.FindAncestor, typeof(Border), 1)
				}
			);
			l2.SetBinding(Line.Y2Property,
				new Binding("ActualHeight") {
					Mode = BindingMode.OneWay,
					RelativeSource = new RelativeSource(
						RelativeSourceMode.FindAncestor, typeof(Border), 1)
				}
			);
			c.Children.Add(l2);
			return c;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
