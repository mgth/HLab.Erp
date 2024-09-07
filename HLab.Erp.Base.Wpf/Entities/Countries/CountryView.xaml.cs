using System.Windows.Controls;
using HLab.Erp.Base.Countries;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Base.Wpf.Entities.Countries;

/// <summary>
/// Logique d'interaction pour CountryView.xaml
/// </summary>
public partial class CountryView : UserControl, IView<CountryViewModel>, IDocumentViewClass
{
    public CountryView()
    {
        InitializeComponent();
    }
}