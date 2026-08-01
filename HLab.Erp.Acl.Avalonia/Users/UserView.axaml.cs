using Avalonia.Controls;
using HLab.Erp.Acl.Users;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Avalonia.Users;

/// <summary>
/// Logique d'interaction pour UserView.axaml
/// </summary>
public partial class UserView : UserControl, IView<DefaultViewMode, UserViewModel>, IDocumentViewClass
{
    public UserView()
    {
        InitializeComponent();
    }
}
