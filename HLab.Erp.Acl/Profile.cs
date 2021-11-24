using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

using NPoco;

namespace HLab.Erp.Acl
{
    using H = HD<Profile>;

    public class Profile : Entity, IListableModel
    {
        public Profile() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New profile}":e.Name)
        );

        public string IconPath => throw new System.NotImplementedException();

        readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));
    }
}
