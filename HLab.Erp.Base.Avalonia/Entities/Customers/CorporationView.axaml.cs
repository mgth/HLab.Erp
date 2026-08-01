using Avalonia.Controls;
// ICorporationViewModel vit dans le projet partagé HLab.Erp.Base sous un namespace Wpf (résidu)
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Base.Avalonia.Entities.Customers;

/// <summary>
/// Logique d'interaction pour CorporationView.axaml
/// </summary>
public partial class CorporationView : UserControl, IView<ICorporationViewModel>, IDocumentViewClass
{
    public CorporationView()
    {
        InitializeComponent();
    }
}
