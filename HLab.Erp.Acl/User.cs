using System;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    using H = HD<User>;

    public class User : Entity, IListableModel
    {
        public User() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string FirstName
        {
            get => _firstName.Get();
            set => _firstName.Set(value);
        }
        private readonly IProperty<string> _firstName = H.Property<string>(c => c.Default(""));

        public string Initials
        {
            get => _initials.Get();
            set => _initials.Set(value);
        }
        private readonly IProperty<string> _initials = H.Property<string>(c => c.Default(""));

        public string Login
        {
            get => _login.Get();
            set => _login.Set(value);
        }
        private readonly IProperty<string> _login = H.Property<string>(c => c.Default(""));
        public string Domain
        {
            get => _domain.Get();
            set => _domain.Set(value);
        }
        private readonly IProperty<string> _domain = H.Property<string>(c => c.Default(""));

        public string HashedPassword
        {
            get => _hashedPassword.Get();
            set => _hashedPassword.Set(value);
        }
        private readonly IProperty<string> _hashedPassword = H.Property<string>(c => c.Default(""));


        public string Function
        {
            get => _function.Get();
            set => _function.Set(value);
        }
        private readonly IProperty<string> _function = H.Property<string>(c => c.Default(""));

        public string Phone
        {
            get => _phone.Get();
            set => _phone.Set(value);
        }
        private readonly IProperty<string> _phone = H.Property<string>(c => c.Default(""));

        public string Email
        {
            get => _email.Get();
            set => _email.Set(value);
        }
        private readonly IProperty<string> _email = H.Property<string>(c => c.Default(""));

        public String Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));

        public string Pin
        {
            get => _pin.Get();
            set => _pin.Set(value);
        }
        private readonly IProperty<string> _pin = H.Property<string>(c => c.Default(""));

        public DateTime? Expiry
        {
            get => _expiry.Get();
            set => _expiry.Set(value);
        }
        private readonly IProperty<DateTime?> _expiry = H.Property<DateTime?>();

        [Ignore]
        public string Caption => FirstName + " " + Name + " (" + Initials + ")";
        [Ignore]
        public string IconPath => "Icon/User";

        public static User DesignModel => new User
        {
            Name = "Ouedraogo",
            FirstName = "Michel",
            Initials = "MO",
            Login="o.ouedraogo",
            Domain="hlab.org",
            Function = "Technician",
            Phone = "+200 547 684",
            Email = "o.ouedraogo@hlab.org",

        };
    }
}
