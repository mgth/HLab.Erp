using System;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    public class LocalizeBootloader : IBootloader
    {
        private readonly Func<LocalizeFromDb> _get;
       private readonly ILocalizationService _service;

        public LocalizeBootloader(Func<LocalizeFromDb> get, ILocalizationService service)
        {
            _get = get;
            _service = service;
        }

        public void Load(IBootContext b)
        {
            _service.Register(_get());
        }
    }
}
