using System;
using System.Collections.Generic;
using System.Text;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{
    public interface IErpServices
    {
        IDataService Data { get; }
        IMvvmService Mvvm { get; }
        IDocumentService Docs { get; }
        IAclService Acl { get; }
        IMessageBus Message { get;  }
        IMenuService Menu { get; }
        IIconService Icon { get; }
    }

    [Export(typeof(IErpServices))]
    class ErpServices : IErpServices
    {
        [Import] public ErpServices(IDataService data, IMvvmService mvvm, IDocumentService docs, IAclService acl, IMessageBus message, IMenuService menu, IIconService icon)
        {
            Data = data;
            Mvvm = mvvm;
            Docs = docs;
            Acl = acl;
            Message = message;
            Menu = menu;
            Icon = icon;
        }

        public IDataService Data { get; }
        public IMvvmService Mvvm { get; }
        public IDocumentService Docs { get; }
        public IAclService Acl { get; }
        public IMessageBus Message { get; }
        public IMenuService Menu { get; }
        public IIconService Icon { get; }
    }
}
