using System;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Icons;


namespace HLab.Erp.Core.Wpf.WebService
{
    public class BrowserServiceModule : IPostBootloader
    {
        [Import]
        private readonly IMenuService _menus;
        [Import]
        private readonly IIconService _icons;
        [Import]
        private readonly IDocumentService _docs;
        [Import]
        private readonly Func<Action, Func<bool>, ICommand> _getCommand;


        public void Load()
        {
            var cmd = _getCommand(() => _docs.OpenDocument(typeof(IBrowserService)), () => true);
            _menus.RegisterMenu("tools", "internet", "Internet", cmd, _icons.GetIcon("icons/internet"));
        }
    }
}
