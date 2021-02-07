using System.Windows.Controls;

using HLab.Base.Wpf;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

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
