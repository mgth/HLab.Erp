using System;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists
{




    public class Column<T> : IColumn<T>
    {
        internal Column()
        {
            Id = "C" + Guid.NewGuid().ToString().Replace('-', '_');
        }

        public bool Hidden { get; set; } = false;

        public object Header { get; set; } = "";
        public string IconPath { get; set; } = "";

        public double Width { get; set; } = double.NaN;

        public Func<T, object> OrderBy { get;  set; }

        public SortDirection SortDirection { get;  set; }

        public Func<T, object> Getter { get; set; }

        public IColumn<T> OrderByNext { get; set; }

        public object GetValue(T value)
        {
            #if DEBUG
            if (Getter is null) return "<Null>";
            #endif

            try
            {
                return Getter(value);
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return Header.ToString();
        }
    }
}