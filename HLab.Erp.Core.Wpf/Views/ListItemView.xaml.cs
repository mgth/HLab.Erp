using System.Windows.Controls;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour ListItemView.xaml
    /// </summary>
    public partial class ListItemView : UserControl
        ,IView<ListViewMode,IListItemViewModel>
    {
        public ListItemView()
        {
            InitializeComponent();
        }
    }
}
