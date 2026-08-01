using Avalonia.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Avalonia.Lists;

/// <summary>
/// Logique d'interaction pour ListableView.axaml
/// </summary>
public partial class ListableView : UserControl, IView<IListableModel>, IListItemViewClass
{
    public ListableView()
    {
        InitializeComponent();
    }
}
