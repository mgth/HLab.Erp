using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    public interface ICorporation
    {
        string Name { get; set; }
        string Address { get; set; }
        string Phone { get; set; }
        string Fax { get; set; }
        string Email { get; set; }
        string Note { get; set; }

        int? CountryId { get; set; }
        Country Country { get; set; }
    }

    public abstract class Corporation<T> : Entity<T>, ICorporation 
        where T : Corporation<T>
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string Address
        {
            get => _address.Get();
            set => _address.Set(value);
        }

        private readonly IProperty<string> _address = H.Property<string>(c => c.Default(""));
        public string Phone
        {
            get => _phone.Get();
            set => _phone.Set(value);
        }

        private readonly IProperty<string> _phone = H.Property<string>(c => c.Default(""));
        public string Fax
        {
            get => _fax.Get();
            set => _fax.Set(value);
        }

        private readonly IProperty<string> _fax = H.Property<string>(c => c.Default(""));
        public string Email
        {
            get => _email.Get();
            set => _email.Set(value);
        }
        private readonly IProperty<string> _email = H.Property<string>(c => c.Default(""));
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));
        public int? CountryId
        {
            get => _country.Id.Get();
            set => _country.Id.Set(value);
        }

        [Ignore] public Country Country
        {
            get => _country.Get();
            set => _country.Set(value);
        }
        private readonly IForeign<Country> _country = H.Foreign<Country>();
    }

    public class Customer : Corporation<Customer>, ILocalCache, IListableModel
    {
        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .On(e => e.Id)
            //TODO : localize
            .Set(e => (e.Id < 0 && string.IsNullOrEmpty(e.Name)) ? "Nouveau client" : e.Name)
        );

        [Ignore] public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .OneWayBind(e => e.Country.IconPath)
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
