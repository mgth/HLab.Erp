using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Data
{
    public class Icon : Entity<Icon>
    {
        [Column]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        [Column]
        public string Source
        {
            get => _source.Get();
            set => _source.Set(value);
        }
        private readonly IProperty<string> _source = H.Property<string>(c => c.Default(""));

        [Column]
        public string Format
        {
            get => _format.Get();
            set => _format.Set(value);
        }
        private readonly IProperty<string> _format = H.Property<string>(c => c.Default(""));
    }
}
