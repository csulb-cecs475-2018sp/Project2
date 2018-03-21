using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.WpfView {
	/// <summary>
	/// Encapsulates a view model used to build a WPF Control to view and interact with a particular game.
	/// </summary>
	public interface IWpfGameView {
		System.Windows.Controls.Control ViewControl { get; }
		IGameViewModel ViewModel { get; }
	}
}
