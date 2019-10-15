using System;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Localization
{
    public class LocalizeBootloader : IBootloader
    {
        [Import]
        private Func<LocalizeFromDb> _get;
        [Import]
        private ILocalizationService _service;

        public void Load()
        {
            _service.Register(_get());
        }
    }

    public class LocalizeWpfBootloader : IPostBootloader
    {
        [Import]
        private readonly IIconService _icons;
        [Import]
        private readonly IMenuService _menu;
        [Import]
        private readonly IDocumentService _docs;
        [Import]
        private readonly ICommandService _command;
        public void Load()
        {
            var cmd = _command.Get(() => _docs.OpenDocument(typeof(LocalizationViewModel)), () => true);

            _menu.RegisterMenu("tools", "localization", "Localization",
            cmd,
            _icons.GetIcon("icons/localize"));
        }
    }
}
