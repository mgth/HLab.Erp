using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core
{
    public class ErpServices : IErpServices
    {
        public ErpServices(
            ILocalizationService localization, 
            IDataService data, IMvvmService mvvm, 
            IDocumentService docs, 
            IAclService acl, 
            IMessageBus message, 
            IMenuService menu, 
            IApplicationInfoService info)
        {
            Localization = localization;
            Data = data;
            Mvvm = mvvm;
            Docs = docs;
            Acl = acl;
            Message = message;
            Menu = menu;
            Info = info;
        }

        public IApplicationInfoService Info { get; }
        public IDataService Data { get; }
        public IMvvmService Mvvm { get ; }
        public IDocumentService Docs { get; }
        public IAclService Acl { get; }
        public IMessageBus Message { get; }
        public IMenuService Menu { get; }
        public ILocalizationService Localization {get; }


        public ServiceState ServiceState
        {
            get
            {
                if(
                    
                    Data.ServiceState == ServiceState.Available
                    //&& Info
                    && Mvvm.ServiceState == ServiceState.Available
                    //&& Docs
                    && Acl.ServiceState == ServiceState.Available
                    //&& Message
                    //&& Menu
                    ) return ServiceState.Available;

                return ServiceState.NotConfigured;
            }
        }
    }
}
