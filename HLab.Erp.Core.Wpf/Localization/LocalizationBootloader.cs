using System;
using HLab.Core.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    public class LocalizeBootloader : IBootloader
    {
        private readonly Func<LocalizeFromDb> _get;
       private readonly ILocalizationService _service;
       private readonly IDataService _data;

        public LocalizeBootloader(IDataService data, Func<LocalizeFromDb> get, ILocalizationService service)
        {
            _data = data;
            _get = get;
            _service = service;
        }

        public void Load(IBootContext b)
        {
            if(b.WaitService(_data)) return;

            _service.Register(_get());
        }
    }
}
