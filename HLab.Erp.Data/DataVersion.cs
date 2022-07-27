using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Data
{
    using H = H<DataVersion>;

    public class DataVersion : Entity
    {
        public DataVersion() => H.Initialize(this);

        public string Module
        {
            get => _module.Get();
            set => _module.Set(value);
        }

        readonly IProperty<string> _module = H.Property<string>(c => c.Default(""));
        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }

        readonly IProperty<string> _version = H.Property<string>();

    }
}
