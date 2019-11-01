using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    public class Customer : Entity<Customer>, ILocalCache, IListableModel
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

/*
//        [Column]
 //       public string Contact
 //       {
 //           get => N.Get(() => ""); set => N.Set(value);
//        }


        [Column("ContactTelephone")]
        public string ContactPhone
        {
            get => N.Get(() => ""); set => N.Set(value);
        }
        

        [Column]
        public string ContactEmail
        {
            get => N.Get(() => ""); set => N.Set(value);
        }
*/

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

        [Ignore]
        public string Caption => Name;

        [Ignore]
        public string IconPath => "Icons/Entities/Customer";
    }
}
