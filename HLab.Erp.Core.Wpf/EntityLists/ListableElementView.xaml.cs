using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists
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
