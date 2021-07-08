using System.Windows.Controls;
using HLab.Erp.Acl.Users;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Acl
{
    /// <summary>
    /// Logique d'interaction pour ImportUsers.xaml
    /// </summary>
    public partial class ImportUsers : UserControl, IView<ImportUsersViewModel>, IViewClassDocument
    {
        public ImportUsers()
        {
            InitializeComponent();
        }
    }
}
