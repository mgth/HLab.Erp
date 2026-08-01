using Avalonia.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Avalonia;

/// <summary>
/// Logique d'interaction pour DataLockerView.axaml
/// </summary>
public partial class DataLockerView : UserControl, IView<IDataLocker>
{
    public DataLockerView()
    {
        InitializeComponent();
    }
}
