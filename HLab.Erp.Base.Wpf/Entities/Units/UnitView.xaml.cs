using System.Windows.Controls;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Wpf.Entities.Units
{
    /// <summary>
    /// Logique d'interaction pour UnitView.xaml
    /// </summary>
    public partial class UnitView : UserControl, IView<UnitViewModel>, IViewClassDetail, IViewClassDocument
    {
        public UnitView()
        {
            InitializeComponent();
        }
    }
}
