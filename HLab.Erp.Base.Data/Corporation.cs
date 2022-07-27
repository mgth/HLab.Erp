using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    public abstract class Corporation : Entity, ICorporation 
    {
        protected Corporation() => HD<Corporation>.Initialize(this);
        

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = HD<Corporation>.Property<string>(c => c.Default(""));
        public string Address
        {
            get => _address.Get();
            set => _address.Set(value);
        }

        readonly IProperty<string> _address = HD<Corporation>.Property<string>(c => c.Default(""));
        public string Phone
        {
            get => _phone.Get();
            set => _phone.Set(value);
        }

        readonly IProperty<string> _phone = HD<Corporation>.Property<string>(c => c.Default(""));
        public string Fax
        {
            get => _fax.Get();
            set => _fax.Set(value);
        }

        readonly IProperty<string> _fax = HD<Corporation>.Property<string>(c => c.Default(""));
        public string Email
        {
            get => _email.Get();
            set => _email.Set(value);
        }

        readonly IProperty<string> _email = HD<Corporation>.Property<string>(c => c.Default(""));
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }

        readonly IProperty<string> _note = HD<Corporation>.Property<string>(c => c.Default(""));
        public int? CountryId
        {
            get => _country.Id.Get();
            set => _country.Id.Set(value);
        }

        public Country Country
        {
            get => _country.Get();
            set => _country.Set(value);
        }

        readonly IForeign<Country> _country = HD<Corporation>.Foreign<Country>();
    }
}