using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core
{
    public class ErpServices : IErpServices
    {
        public void Inject(
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

        public IApplicationInfoService Info { get; private set;}
        public IDataService Data { get; private set; }
        public IMvvmService Mvvm { get ; private set;}
        public IDocumentService Docs { get; private set;}
        public IAclService Acl { get; private set;}
        public IMessageBus Message { get; private set;}
        public IMenuService Menu { get; private set;}
        public ILocalizationService Localization {get; private set;}


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
