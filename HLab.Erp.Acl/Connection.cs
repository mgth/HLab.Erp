using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    using H = H<Connection>;
    //[Key("Id", AutoIncrement = false)]
    //[SoftIncrementAttribut]
    public class Connection : Entity
    {
        public Connection() => H.Initialize(this);

        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }

        readonly IProperty<int?> _userId = H.Property<int?>();

        [Ignore]
        public User User
        {
            get => _user.Get();
            set => UserId = value.Id;
        }

        readonly IProperty<User> _user = H.Property<User>(c => c
            .Foreign(e => e.UserId));

        public string Account
        {
            get => _account.Get();
            set => _account.Set(value);
        }

        readonly IProperty<string> _account = H.Property<string>(c => c.Default(""));

        public string Domain
        {
            get => _domain.Get();
            set => _domain.Set(value);
        }

        readonly IProperty<string> _domain = H.Property<string>(c => c.Default(""));

        public string Workstation
        {
            get => _workstation.Get();
            set => _workstation.Set(value);
        }

        readonly IProperty<string> _workstation = H.Property<string>(c => c.Default(""));

        public string Os
        {
            get => _os.Get();
            set => _os.Set(value);
        }
        readonly IProperty<string> _os = H.Property<string>(c => c.Default(""));

        public string Framework
        {
            get => _framework.Get();
            set => _framework.Set(value);
        }
        readonly IProperty<string> _framework = H.Property<string>(c => c.Default(""));

        [Column]
        public string Exe
        {
            get => _exe.Get();
            set => _exe.Set(value);
        }
        readonly IProperty<string> _exe = H.Property<string>(c => c.Default(""));

        [Column]
        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }
        readonly IProperty<string> _version = H.Property<string>(c => c.Default(""));

        [Column]
        public bool X64
        {
            get => _x64.Get();
            set => _x64.Set(value);
        }

        readonly IProperty<bool> _x64 = H.Property<bool>(c => c.Default(true));

        public int Notify
        {
            get => _notify.Get();
            set => _notify.Set(value);
        }

        readonly IProperty<int> _notify = H.Property<int>();
    }
}
