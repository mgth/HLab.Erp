using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Data.Observables
{
    class ObservableQueryConfig : IConfigureInjection
    {
            public void Configure(IExportLocatorScope container)
            {
                //container.Configure(c => c
                //    .ExportWrapper(typeof(ObservableQuery<>))
                //);
            }
    }
}
