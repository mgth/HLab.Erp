using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Module
{
    public class UsersModule : IPostBootloader
    {
        [Import]
        private readonly IIconService _icons;
        [Import]
        private readonly IDbService _db;
        [Import]
        private readonly IMenuService _menu;
        [Import]
        private readonly IDocumentService _docs;
        [Import]
        private readonly ICommandService _command;

        public void Load()
        {
            var cmd = _command.Get(() => _docs.OpenDocument(typeof(ListUsersViewModel)), () => true);

            _menu.RegisterMenu("tools", "users", "Users",
                cmd,
                _icons.GetIcon("icons/Users"));
        }
    }
}
