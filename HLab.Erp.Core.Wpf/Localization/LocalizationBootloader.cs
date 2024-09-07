using System;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Localization;

public class LocalizeBootloader(IDataService data, Func<LocalizeFromDb> get, ILocalizationService service)
    : IBootloader
{
    public Task LoadAsync(IBootContext bootstrapper)
    {
        if(bootstrapper.WaitingForService(data)) return Task.CompletedTask;

        service.Register(get());
        return Task.CompletedTask;
    }
}