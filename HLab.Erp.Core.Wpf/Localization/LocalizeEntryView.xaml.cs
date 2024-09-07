using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Core.Wpf.Localization
{ 
    /// <summary>
    /// Logique d'interaction pour CountryView.xaml
    /// </summary>
    public partial class LocalizeEntryView : UserControl, IView<LocalizeEntryViewModel>, IDocumentViewClass
    {
        public LocalizeEntryView()
        {
            InitializeComponent();
        }
    }
}
