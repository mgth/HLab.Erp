using System.Collections.Specialized;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core
{
    public interface IErpServices
    {
        IApplicationInfoService Info { get; }
        IDataService Data { get; }
        IMvvmService Mvvm { get; }
        IDocumentService Docs { get; }
        IAclService Acl { get; }
        IMessageBus Message { get;  }
        IMenuService Menu { get; }
//        INotifyCollectionChanged Countries{ get; }
        ILocalizationService Localization {get;}
    }

    [Export(typeof(IErpServices))]
    public class ErpServices : IErpServices
    {
        [Import] public ErpServices(ILocalizationService localization, IDataService data, IMvvmService mvvm, IDocumentService docs, IAclService acl, IMessageBus message, IMenuService menu, IApplicationInfoService info/*, ObservableQuery<Country> countries*/)
        {
            Localization = localization;
            Data = data;
            Mvvm = mvvm;
            Docs = docs;
            Acl = acl;
            Message = message;
            Menu = menu;
            Info = info;
//            Countries = countries;
        }

        public IApplicationInfoService Info { get; }
        public IDataService Data { get; }
        public IMvvmService Mvvm { get; }
        public IDocumentService Docs { get; }
        public IAclService Acl { get; }
        public IMessageBus Message { get; }
        public IMenuService Menu { get; }

//        public INotifyCollectionChanged Countries{ get; }

        public ILocalizationService Localization {get; }
    }
}
