using Avalonia.Controls;
using HLab.Erp.Base.Countries;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Base.Avalonia.Entities.Countries;

/// <summary>
/// Logique d'interaction pour CountryView.axaml
/// </summary>
public partial class CountryView : UserControl, IView<CountryViewModel>, IDocumentViewClass
{
    public CountryView()
    {
        InitializeComponent();
    }
}
