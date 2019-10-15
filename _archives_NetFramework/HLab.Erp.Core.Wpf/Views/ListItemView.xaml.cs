using System.Windows.Controls;
using HLab.Erp.Core.Wpf.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour ListItemView.xaml
    /// </summary>
    public partial class ListItemView : UserControl
        ,IView<ViewModeList,IListItemViewModel>
    {
        public ListItemView()
        {
            InitializeComponent();
        }
    }
}
