using Cecs475.BoardGames.Model;
using System;
using System.ComponentModel;

namespace Cecs475.BoardGames.WpfView {
	/// <summary>
	/// Represents a view model for a particular game type, providing functionality for 
	/// the WPF application to use to communicate with the game's model.
	/// </summary>
	public interface IGameViewModel : INotifyPropertyChanged
	{
		GameAdvantage BoardAdvantage { get; }
		int CurrentPlayer { get; }
		bool CanUndo { get; }
		void UndoMove();
		event EventHandler GameFinished;
	}
}
