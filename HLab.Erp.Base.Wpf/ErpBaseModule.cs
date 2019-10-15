using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Icons;

namespace HLab.Erp.Base.Wpf
{
    public class ErpBaseModule : IPostBootloader
    {
        [Import]
        private readonly IIconService _icons;
        [Import]
        private readonly IDbService _db;
        [Import]
        private readonly IMenuService _menu;
        [Import]
        private readonly IDocumentService _docs;
        [Import]
        private readonly ICommandService _command;

        public void Load()
        {
            var cmdCustomer = _command.Get(() => _docs.OpenDocument(typeof(ListCustomerViewModel)), () => true);

            _menu.RegisterMenu("data", "customer", "Customer",
                cmdCustomer,
                _icons.GetIcon("icons/Customer"));

            var cmdCountry = _command.Get(() => _docs.OpenDocument(typeof(ListCountryViewModel)), () => true);

            _menu.RegisterMenu("data", "country", "Country",
                cmdCountry,
                _icons.GetIcon("icons/Country"));

            var cmdIcon = _command.Get(() => _docs.OpenDocument(typeof(ListIconViewModel)), () => true);

            _menu.RegisterMenu("tools", "icons", "Icons",
                cmdIcon,
                _icons.GetIcon("icons/Icon"));
        }
    }
}
