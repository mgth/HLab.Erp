using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    using H = HD<LocalizeEntry>;

    public class LocalizeEntry : Entity, ILocalizeEntry, IListableModel
    {
        public LocalizeEntry() => H.Initialize(this);

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
        public bool Custom
        {
            get => _custom.Get();
            set => _custom.Set(value);
        }
        private readonly IProperty<bool> _custom = H.Property<bool>(c => c.Default(false));

        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .Set(e => string.IsNullOrWhiteSpace(e.Code)?"{New localize entry}":$"{e.Tag} - {e.Code}")
            .On(e => e.Code)
            .On(e => e.Tag)
            .Update()
        );

    }
}
