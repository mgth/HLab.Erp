using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Wcf.KioskLogin
{
    /// <summary>
    /// Logique d'interaction pour UserView.xaml
    /// </summary>
    public partial class UserView : UserControl, IView<ViewModeDefault,User>, IViewClassListItem
    {
        public UserView()
        {
            InitializeComponent();
        }
    }
}
