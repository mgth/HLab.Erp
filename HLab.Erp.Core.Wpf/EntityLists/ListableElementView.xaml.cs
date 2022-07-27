using System.Windows.Controls;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    /// <summary>
    /// Logique d'interaction pour ListableElementView.xaml
    /// </summary>
    public partial class ListableElementView : UserControl, IView<IListableModel>, IListElementViewClass
    {
        public ListableElementView()
        {
            InitializeComponent();
        }
    }
}
