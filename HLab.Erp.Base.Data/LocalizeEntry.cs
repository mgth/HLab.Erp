using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;


namespace HLab.Erp.Base.Data
{
    public class LocalizeEntry : Entity<LocalizeEntry>, ILocalizeEntry
    {
        public string Tag
        {
            get => _tag.Get();
            set => _tag.Set(value);
        }
        private readonly IProperty<string> _tag = H.Property<string>(c => c.Default("en-us"));

        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }
        private readonly IProperty<string> _code = H.Property<string>(c => c.Default(""));

        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<string> _value = H.Property<string>(c => c.Default(""));

        public bool Todo
        {
            get => _todo.Get();
            set => _todo.Set(value);
        }
        private readonly IProperty<bool> _todo = H.Property<bool>(c => c.Default(true));

        public bool BadCode
        {
            get => _badCode.Get();
            set => _badCode.Set(value);
        }
        private readonly IProperty<bool> _badCode = H.Property<bool>(c => c.Default(false));
    }
}
