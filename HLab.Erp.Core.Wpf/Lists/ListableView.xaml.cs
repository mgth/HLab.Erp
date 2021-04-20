using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Lists
{
    /// <summary>
    /// Logique d'interaction pour ListableView.xaml
    /// </summary>
    public partial class ListableView : UserControl, IView<IListableModel>, IViewClassListItem
    {
        public ListableView()
        {
            InitializeComponent();
        }
    }
}
