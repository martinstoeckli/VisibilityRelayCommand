# VisibilityRelayCommand
RelayCommand which manages the visibility of the command.

If working with C# MVVM you surely already used a RelayCommand, e.g. the implementation from the [CommunityToolkit](https://github.com/MicrosoftDocs/CommunityToolkit/blob/main/docs/mvvm/RelayCommand.md). The RelayCommand can update the `Enabled` state of the XAML components automatically if they have a binding to the command. What if we could do the same with the `Visibility` of the components?

This repository is about a single class [VisibilityRelayCommand.cs](VisibilityRelayCommandDemo/VisibilityRelayCommandDemo/VisibilityRelayCommand.cs) which offers an additional `Visibility` property and handles the notification automatically, so you don't need to add an extra "IsMyCommandVisible" property the ViewModel.

## Usage

In the *.xaml view bind the command as usual, but also bind the Visibility property to the commands Visibility property. Note that there is no need for the `BooleanToVisibilityConverter` just bind directly.

```Xml
<Button
    Content="Run the command"
    Command="{Binding RunCommand}"
    CommandParameter="Whatever"
    Visibility="{Binding RunCommand.Visibility}" />
```

In the ViewModel create the command and pass a delegate as the second parameter. In contrast to the `CanExecute()` this is an action which gets a state as parameter, and there you can set the `IsEnabled` as well as the `IsVisible` property. This values will then be applied to all components with a binding to this command.

```csharp
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
    state.IsEnabled = ...; // Set the value you would return in CanExecute()
    state.IsVisible = ...; // Determine the visibility
}
```

Because the `VisibilityRelayCommand` is taking advantage of the CommandManager of the  DotNet framework, which asks for the `CanExecute` state whenever necessary, there is no need for us to find all points where a notification of the visibility must be done.

## Remarks
This should just give you the idea, how the visibility can be controlled by a Command as it already does for the enabled state. The class could of course be extended with generic implementations as shown in the CommunityToolkit. Unfortunately the RelayCommand of the CommunityToolkit is sealed, otherwise I would have inherited from their implementation.
