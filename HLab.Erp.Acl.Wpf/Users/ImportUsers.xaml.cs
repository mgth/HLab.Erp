using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Users;

/// <summary>
/// Logique d'interaction pour ImportUsers.xaml
/// </summary>
public partial class ImportUsers : UserControl, IView<ImportUsersViewModel>, IDocumentViewClass
{
    public ImportUsers()
    {
        InitializeComponent();
    }
}