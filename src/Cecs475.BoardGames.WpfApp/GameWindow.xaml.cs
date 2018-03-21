using Cecs475.BoardGames.WpfView;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cecs475.BoardGames.WpfApp {
	public partial class GameWindow : Window {
		/// <summary>
		/// Creates a GameWindow containing the given game View control.
		/// </summary>
		public GameWindow(IWpfGameFactory factory) {
			// Set the DynamicResource named GameView to the given control.
			var gameView = factory.CreateGameView();
			this.Resources.Add("GameView", gameView.ViewControl);
			this.Resources.Add("ViewModel", gameView.ViewModel);

			// Register an event handler for when the game is finished.
			gameView.ViewModel.GameFinished += ViewModel_GameFinished;

			InitializeComponent();

			// Set up bindings manually -- there are ways to do this in XAML, but I want to demonstrate the C# equivalent. 
			mAdvantageLabel.SetBinding(Label.ContentProperty,
				new Binding() {
					Path = new PropertyPath("BoardAdvantage"),
					Converter = factory.CreateBoardAdvantageConverter()
				}
			);

			mPlayerLabel.SetBinding(Label.ContentProperty,
				new Binding() {
					Path = new PropertyPath("CurrentPlayer"),
					Converter = factory.CreateCurrentPlayerConverter()
				}
			);
		}

		// Invoked when a game is complete, as reported by its view model.
		private void ViewModel_GameFinished(object sender, EventArgs e) {
			MessageBox.Show("Game over!");
		}

		private void UndoButton_Click(object sender, RoutedEventArgs e) {
			(FindResource("ViewModel") as IGameViewModel).UndoMove();
		}
	}
}
