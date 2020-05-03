using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Users
{
    /// <summary>
    /// Logique d'interaction pour UserView.xaml
    /// </summary>
    public partial class UserView : UserControl, IView<ViewModeDefault,UserViewModel>, IViewClassDocument
    {
        public UserView()
        {
            InitializeComponent();
        }
    }
}
