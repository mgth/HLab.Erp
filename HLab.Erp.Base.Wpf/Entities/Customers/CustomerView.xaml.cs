using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    /// <summary>
    /// Logique d'interaction pour CustomerView.xaml
    /// </summary>
    public partial class CorporationView : UserControl, IView<ICorporationViewModel>, IViewClassDocument
    {
        public CorporationView()
        {
            InitializeComponent();
        }
    }
}
