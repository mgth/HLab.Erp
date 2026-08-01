using Avalonia.Controls;
using HLab.Erp.Acl.Profiles;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Avalonia.Profiles;

/// <summary>
/// Logique d'interaction pour ProfileView.axaml
/// </summary>
public partial class ProfileView : UserControl, IView<ProfileViewModel>, IDocumentViewClass, IDetailViewClass
{
    public ProfileView()
    {
        InitializeComponent();
    }
}
