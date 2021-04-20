using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    using H = H<Customer>;

    public class Customer : Corporation, ILocalCache, IListableModel
    {
        public Customer() => H.Initialize(this); 


        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .On(e => e.Id)
            .Set(e => (e.Id < 0 && string.IsNullOrEmpty(e.Name)) ? "{New customer}" : e.Name)
        );

        [Ignore] public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .Bind(e => e.Country.IconPath)
        );

        public static Customer GetDesignModel()
        {
            return new Customer
            {
                Name = "Dummy Customer",
                Address = "Somewhere in the world\n10000 NOWHERE",
                Phone = "+33 6 123 123"
            };
        }
    }
}
