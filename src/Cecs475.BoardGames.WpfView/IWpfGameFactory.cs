using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cecs475.BoardGames.WpfView {
	/// <summary>
	/// Represents a game type that can be played in our WPF application, providing factory methods
	/// to construct views and converters for the game.
	/// </summary>
	public interface IWpfGameFactory {
		string GameName { get; }
		IWpfGameView CreateGameView();
		IValueConverter CreateBoardAdvantageConverter();
		IValueConverter CreateCurrentPlayerConverter();
	}
}
