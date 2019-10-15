using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    /// <summary>
    /// Logique d'interaction pour LocalizationView.xaml
    /// </summary>
    public partial class LocalizationView : UserControl, IView<LocalizationViewModel>, IViewClassDocument
    {
        public LocalizationView()
        {
            InitializeComponent();
        }
    }
}
