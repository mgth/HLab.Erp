﻿using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class ImportUsersModule : N<ImportUsersModule>, IBootloaderDependent
    {
        [Import]
        private readonly IErpServices _erp;

        public string[] DependsOn => new []{"BootLoaderErpWpf"};

        public ICommand OpenCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ImportUsersViewModel))
        ).CanExecute(e => true));

        protected virtual string IconPath => "Icons/Entities/";

        public virtual void Load(IBootContext b)
        {
            if (_erp.Acl.Connection == null)
            {
                b.Requeue();
                return;
            }
            if(!_erp.Acl.IsGranted(AclRights.ManageUser)) return;

            _erp.Menu.RegisterMenu("tools/ImportUsers", "{Import Users}",
                OpenCommand,
                "icons/tools/ImportUsers");
        }
    }
}
