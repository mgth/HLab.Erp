using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Wpf.Entities.Users;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Module.UsersModule>;
namespace HLab.Erp.Module
{
    public class UsersModule : IPostBootloader
    {
        [Import]
        private readonly IErpServices _erp;

        public UsersModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand ListUsersOpenDocumentCommand { get; } = H.Command(c => c
            .Action(e => e._erp.Docs.OpenDocumentAsync(typeof(ListUserViewModel)))
        );

        public void Load()
        {
            _erp.Menu.RegisterMenu("tools", "users", "{Users}",
                ListUsersOpenDocumentCommand,
                "icons/Users");
        }
    }
}
