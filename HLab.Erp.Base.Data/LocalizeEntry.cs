using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Base.Data.LocalizeEntry>;


namespace HLab.Erp.Base.Data
{
    public class LocalizeEntry : Entity<LocalizeEntry>, INotifyPropertyChanged// Entity<LocalizeEntry>
    {
        public LocalizeEntry()
        {
            H.Initialize(this);
        }

        [Column]
        public string Tag
        {
            get => _tag.Get();
            set => _tag.Set(value);
        }
        private readonly IProperty<string> _tag = H.Property<string>(c => c.Default("en-us"));

        [Column]
        public string Code
        {
            get => _code.Get();
            set => _code.Set(value);
        }
        private readonly IProperty<string> _code = H.Property<string>(c => c.Default(""));

        [Column]
        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<string> _value = H.Property<string>(c => c.Default(""));
        [Column]
        public bool Todo
        {
            get => _todo.Get();
            set => _todo.Set(value);
        }
        private readonly IProperty<bool> _todo = H.Property<bool>(c => c.Default(true));
        [Column]
        public bool BadCode
        {
            get => _badCode.Get();
            set => _badCode.Set(value);
        }
        private readonly IProperty<bool> _badCode = H.Property<bool>(c => c.Default(false));
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
