using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Acl.Profiles
{
    /// <summary>
    /// Logique d'interaction pour ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl, IView<ProfileViewModel>, IViewClassDocument, IViewClassDetail
    {
        public ProfileView()
        {
            InitializeComponent();
        }
    }
}
