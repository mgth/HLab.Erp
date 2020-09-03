using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

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
