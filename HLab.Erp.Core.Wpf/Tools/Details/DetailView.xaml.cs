using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Core.Wpf.Tools.Details;

/// <summary>
/// Logique d'interaction pour DetailView.xaml
/// </summary>
public partial class DetailView : UserControl, IAnchorableViewClass, IView<DefaultViewMode, DetailsPanelViewModel>
{
    public DetailView()
    {
        InitializeComponent();
    }

    public string ContentId => GetType().Name;
}