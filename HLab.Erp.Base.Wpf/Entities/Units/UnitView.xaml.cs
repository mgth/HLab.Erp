using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Base.Wpf.Entities.Units;

/// <summary>
/// Logique d'interaction pour UnitView.xaml
/// </summary>
public partial class UnitView : UserControl, IView<UnitViewModel>, IDetailViewClass, IDocumentViewClass
{
    public UnitView()
    {
        InitializeComponent();
    }
}