using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Core.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;
using ReactiveUI;

namespace HLab.Erp.Acl.Users;

public class ImportUsersModule : Bootloader
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

   protected override BootState Load()
   {
      if (WaitingForBootloader("BootLoaderErpWpf")) return BootState.Requeue;

      if (_acl.Connection == null)
      {
         return _acl.Cancelled ? BootState.Cancel : BootState.Requeue;
      }

      if (_acl.IsGranted(AclRights.ManageUser))
      {
         _menu.RegisterMenu("tools/ImportUsers", "{Import Users}",
             OpenCommand,
             "icons/tools/ImportUsers");
      }

      return base.Load();
   }

}