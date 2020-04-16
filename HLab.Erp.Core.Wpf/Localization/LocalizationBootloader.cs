using System;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    public class LocalizeBootloader : IBootloader
    {
        [Import]
        private Func<LocalizeFromDb> _get;
        [Import]
        private ILocalizationService _service;

        public void Load(IBootContext b)
        {
            _service.Register(_get());
        }
    }
}
