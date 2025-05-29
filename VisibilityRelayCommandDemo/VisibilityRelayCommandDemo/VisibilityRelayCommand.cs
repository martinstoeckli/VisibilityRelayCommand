// Copyright © 2025 Martin Stoeckli.
// This source code form is subject to the terms of the MIT
// license. If a copy of the license was not distributed with this
// file, you can obtain one at https://opensource.org/license/mit .

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Sto
{
	/// <summary>
	/// Extends the <see cref="ICommand"/> interface by a <see cref="Visibility"/> property,
	/// which can be used in the XAML for binding.
	/// <example>
	/// Command="{Binding RunCommand}"
	/// Visibility="{Binding RunCommand.Visibility}"
	/// </example>
	/// </summary>
	public interface IVisibilityRelayCommand : ICommand, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the visibility of the command. The notification is done automatically, just bind
		/// it to the XAML "Visibility" property.
		/// </summary>
		public Visibility Visibility { get; }
	}

	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. The default return value for the <see cref="CanExecute"/>
	/// method is <see langword="true"/>. This type does not allow you to accept command parameters
	/// in the <see cref="Execute"/> and <see cref="CanExecute"/> callback methods.
	/// </summary>
	public class VisibilityRelayCommand : IVisibilityRelayCommand
	{
		private readonly Action<object?> _execute;
		private readonly Action<VisibilityRelayCommandState> _getCommandState;
		private Visibility _visibility;

		/// <summary>
		/// Initializes a new instance of the <see cref="VisibilityRelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The action to run when the command is executed.</param>
		/// <param name="getCommandState">This delegate which will be called to get the current
		/// state. Change the parameters <see cref="VisibilityRelayCommandState.IsEnabled"/>
		/// and <see cref="VisibilityRelayCommandState.IsVisible"/> properties if necessary.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="getCommandState"/> are <see langword="null"/>.</exception>
		public VisibilityRelayCommand(Action<object?> execute, Action<VisibilityRelayCommandState> getCommandState)
		{
			ArgumentNullException.ThrowIfNull(execute);
			ArgumentNullException.ThrowIfNull(getCommandState);
			_execute = execute;
			_getCommandState = getCommandState;
			_visibility = Visibility.Visible;
		}

		/// <inheritdoc/>
		public Visibility Visibility
		{
			get { return _visibility; }

			private set
			{
				// Set the new value if different and notify the view.
				if (_visibility != value)
				{
					_visibility = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
				}
			}
		}

		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <inheritdoc />
		public event EventHandler? CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <inheritdoc/>
		public bool CanExecute(object? commandParameter)
		{
			// We take advantage of the CommandManager calling this function frequently, and let it
			// determine not only the "Enabled/CanExecute" state but also the "Visibility".
			var commandState = new VisibilityRelayCommandState(commandParameter);
			_getCommandState.Invoke(commandState);

			// Setting the Visibility property will automatically notify the view.
			Visibility = commandState.IsVisible ? Visibility.Visible : Visibility.Collapsed;

			// Return the actual result of CanExecute.
			return commandState.IsEnabled;
		}

		/// <inheritdoc/>
		public void Execute(object? commandParameter)
		{
			_execute(commandParameter);
		}
	}

	/// <summary>
	/// The parameter passed to the "getCommandState" delegate of the <see cref="VisibilityRelayCommand"/>.
	/// </summary>
	public class VisibilityRelayCommandState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VisibilityRelayCommandState"/> class.
		/// </summary>
		/// <param name="commandParameter">Sets the <see cref="CommandParameter"/> property.</param>
		public VisibilityRelayCommandState(object? commandParameter)
		{
			CommandParameter = commandParameter;
			IsEnabled = true;
			IsVisible = true;
		}

		/// <summary>
		/// Gets the command parameter if defined in the XAML.
		/// </summary>
		public object? CommandParameter { get; }

		/// <summary>
		/// Gets or sets a value indicating whether XAML components with a binding to this
		/// command are enabled or not (CanExecute).
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether XAML components with a binding to this
		/// command are visible or hidden.
		/// </summary>
		public bool IsVisible { get; set; }
	}
}
