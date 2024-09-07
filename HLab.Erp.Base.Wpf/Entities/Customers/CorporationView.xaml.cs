using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Base.Wpf.Entities.Customers;

/// <summary>
/// Logique d'interaction pour CustomerView.xaml
/// </summary>
public partial class CorporationView : UserControl, IView<ICorporationViewModel>, IDocumentViewClass
{
    public CorporationView()
    {
        InitializeComponent();
    }
}