using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Core.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;
using ReactiveUI;

namespace HLab.Erp.Acl.Users;

public class ImportUsersModule : ReactiveModel, IBootloader
{
    readonly IDocumentService _docs;
    readonly IAclService _acl;
    readonly IMenuService _menu;
    public ImportUsersModule(IDocumentService docs, IAclService acl, IMenuService menu)
    {
        _docs = docs;
        _acl = acl;
        _menu = menu;

        OpenCommand = ReactiveCommand.CreateFromTask(async () => await _docs.OpenDocumentAsync(typeof(ImportUsersViewModel)));
    }

    public ICommand OpenCommand { get; }

    protected virtual string IconPath => "Icons/Entities/";

    public async virtual Task LoadAsync(IBootContext b)
    {
        if (b.WaitDependency("BootLoaderErpWpf")) return;

        if (_acl.Connection == null)
        {
            if(!_acl.Cancelled) b.Requeue();
            return;
        }

        if(!_acl.IsGranted(AclRights.ManageUser)) return;

        _menu.RegisterMenu("tools/ImportUsers", "{Import Users}",
            OpenCommand,
            "icons/tools/ImportUsers");
    }

}