using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    /// <summary>
    /// Logique d'interaction pour IconView.xaml
    /// </summary>
    public partial class IconDetailView: UserControl, IView<IconViewModel>
    {
        public IconDetailView()
        {
            InitializeComponent();
        }
    }
}
