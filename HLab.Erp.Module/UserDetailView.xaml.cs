using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core.Wpf.Tools.Details;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Module
{
    /// <summary>
    /// Logique d'interaction pour UserDetailView.xaml
    /// </summary>
    public partial class UserDetailView : UserControl, IView<UserViewModel>, IViewClassDetail
    {
        public UserDetailView()
        {
            InitializeComponent();

            DataContextChanged += UserDetailView_DataContextChanged;
            
        }

        private void UserDetailView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is UserViewModel vm)
            {
                if (vm.Locker.IsActive)
                {
                    var result = MessageBox.Show(
                        "Modification en cours",
                        "Sauvegarder les modifications ?",
                        MessageBoxButton.YesNo, MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                        vm.Locker.IsActive = false;

                }
            }
        }
    }
}
