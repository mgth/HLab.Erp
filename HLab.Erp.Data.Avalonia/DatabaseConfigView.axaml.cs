using Avalonia.Controls;
using Avalonia.Interactivity;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Avalonia;

namespace HLab.Erp.Data.Avalonia;

/// <summary>
/// Logique d'interaction pour DatabaseConfigView.axaml
/// </summary>
public partial class DatabaseConfigView : UserControl, IView<ConnectionDataViewModel>
{
    public DatabaseConfigView()
    {
        InitializeComponent();
    }

    void Close(bool result)
    {
        if (TopLevel.GetTopLevel(this) is not Window window) return;
        if (window is DefaultWindow dw) dw.DialogResult = result;
        window.Close(result);
    }

    void OkButton_OnClick(object? sender, RoutedEventArgs e) => Close(true);

    void CancelButton_OnClick(object? sender, RoutedEventArgs e) => Close(false);
}
