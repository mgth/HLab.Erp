﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Wpf.Entities.Countries;
using HLab.Erp.Base.Wpf.Entities.Icons;
using HLab.Erp.Base.Wpf.Entities.Profiles;
using HLab.Erp.Base.Wpf.Entities.Users;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf
{
    public class ErpBaseModule : N<ErpBaseModule>, IPostBootloader
    {
        
        private readonly IErpServices _erp;

        [Import]public ErpBaseModule(IErpServices erp):base(true)
        {
            _erp = erp;
            Initialize();
        }

        public ICommand CustomerCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListCustomerViewModel))
            ).CanExecute(e => true));

        public ICommand CountryCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocument(typeof(ListCountryViewModel))
            ));
        public ICommand IconCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocument(typeof(ListIconViewModel))
            ));
        public ICommand UserCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocument(typeof(ListUserViewModel))
            ));
        public ICommand ProfileCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocument(typeof(ListProfileViewModel))
            ));
        public void Load()
        {
            _erp.Menu.RegisterMenu("data", "customer", "{Customer}",
                CustomerCommand,
                _erp.Icon.GetIcon("Icons/Entities/Customer"));

            _erp.Menu.RegisterMenu("param", "country", "{Country}",
                CountryCommand,
                _erp.Icon.GetIcon("Icons/Entities/Country"));

            _erp.Menu.RegisterMenu("param", "icons", "{Icons}",
                IconCommand,
                _erp.Icon.GetIcon("Icons/Icon"));

            _erp.Menu.RegisterMenu("param", "users", "{Users}",
                UserCommand,
                _erp.Icon.GetIcon("Icons/Entities/User"));

            _erp.Menu.RegisterMenu("param", "profiles", "{Profiles}",
                ProfileCommand,
                _erp.Icon.GetIcon("Icons/Entities/Profile"));
        }
    }
}
