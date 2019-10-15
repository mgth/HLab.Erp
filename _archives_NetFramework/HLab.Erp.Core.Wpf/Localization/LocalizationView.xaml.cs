using System.Windows.Controls;
using HLab.Erp.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Localization
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
