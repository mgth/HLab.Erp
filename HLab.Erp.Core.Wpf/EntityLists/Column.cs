using System;

namespace HLab.Erp.Core.EntityLists
{




    public class Column<T> : IColumn<T>
    {
        internal Column()
        {
            Id = "C" + Guid.NewGuid().ToString().Replace('-', '_');
        }

        public Column(object caption, Func<T, object> getter,Func<T, object> orderBy, string id, bool hidden)
        {
            Id = id ?? ("C" + Guid.NewGuid().ToString().Replace('-', '_'));
            Header = caption;
            OrderBy = orderBy;
            Getter = getter;
            Hidden = hidden;
        }

        public bool Hidden { get; set; } = false;

        public object Header { get; set; } = "";

        public double Width { get; set; } = double.NaN;

        public Func<T, object> OrderBy { get;  set; }

        public int OrderByOrder { get;  set; }

        public bool OrderDescending { get;  set; }

        public Func<T, object> Getter { get; set; }

        void IColumn<T>.AddGetter(Func<T,object, object> getter)
        {
            Getter = (a) => getter(a, Getter(a));
        }

        public object GetValue(T value)
        {
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
            return Header.ToString() + OrderByOrder.ToString();
        }
    }
}