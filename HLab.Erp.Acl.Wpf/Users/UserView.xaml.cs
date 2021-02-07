using System.Windows.Controls;

using HLab.Base.Wpf;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

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

        private void TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(DataContext is UserViewModel vm &&!vm.Locker.IsActive) vm.Locker.ActivateAsync();
        }

    }
}
