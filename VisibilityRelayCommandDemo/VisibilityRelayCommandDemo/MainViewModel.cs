using System.Windows;
using Sto;

namespace VisibilityRelayCommandDemo
{
	internal class MainViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainViewModel"/> class.
		/// </summary>
		public MainViewModel()
		{
			RunCommand = new VisibilityRelayCommand(Run, GetRunCommandState);
		}

		public IVisibilityRelayCommand RunCommand { get; }

		private void Run(object? obj)
		{
			MessageBox.Show("Command is running");
		}

		private void GetRunCommandState(VisibilityRelayCommandState state)
		{
			state.IsEnabled = ShouldBeEnabled;
			state.IsVisible = ShouldBeVisible;
		}

		/// <summary>
		/// This property has a binding to the "Command is enabled" checkbox,
		/// but it does not do any notification.
		/// </summary>
		public bool ShouldBeEnabled { get; set; } = true;

		/// <summary>
		/// This property has a binding to the "Command is visible" checkbox,
		/// but it does not do any notification.
		/// </summary>
		public bool ShouldBeVisible { get; set; } = true;
	}
}
