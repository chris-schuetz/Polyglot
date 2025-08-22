using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Preferences.Avalonia.ViewModels;

namespace Preferences.Avalonia.Views;

/// <summary>
/// Represents the main window view for displaying and managing application preferences.
/// </summary>
public partial class PreferencesView : Window
{
    public PreferencesView()
    {
        InitializeComponent();
    }

    public PreferencesView(PreferencesViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
