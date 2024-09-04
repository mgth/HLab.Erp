using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    using H = HD<LocalizeEntry>;

    public class LocalizeEntry : Entity, ILocalizeEntry, IListableModel
    {
        public LocalizeEntry() { }

        public string Tag
        {
            get => _tag;
            set => this.RaiseAndSetIfChanged(ref _tag,value);
        }

        string _tag = "en-us";

        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code,value);
        }

        string _code = "";

        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value,value);
        }

        string _value = "";

        public bool Todo
        {
            get => _todo;
            set => this.RaiseAndSetIfChanged(ref _todo,value);
        }

        bool _todo = true;

        public bool BadCode
        {
            get => _badCode;
            set => this.RaiseAndSetIfChanged(ref _badCode,value);
        }

        bool _badCode = false;
        public bool Custom
        {
            get => _custom;
            set => this.RaiseAndSetIfChanged(ref _custom,value);
        }

        bool _custom = false;

        public string Caption => _caption.Get();

        string _caption = H.Property<string>(c => c
            .Set(e => string.IsNullOrWhiteSpace(e.Code)?"{New localize entry}":$"{e.Tag} - {e.Code}")
            .On(e => e.Code)
            .On(e => e.Tag)
            .Update()
        );

    }
}
