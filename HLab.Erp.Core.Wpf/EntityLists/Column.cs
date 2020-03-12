using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core.EntityLists
{
    public class Column<T>
    {
        public Column(object caption, Func<T, object> getter,Func<T, object> orderBy, string id, bool hidden)
        {
            Id = id ?? ("C" + Guid.NewGuid().ToString().Replace('-', '_'));
            Caption = caption;
            OrderBy = orderBy;
            _getter = getter;
            Hidden = hidden;
        }

        public bool Hidden { get; }
        public object Caption { get; }
        public Func<T, object> OrderBy { get; }
        private readonly Func<T, object> _getter;

        public object Get(T value)
        {
            try
            {
                return _getter(value);
            }
            catch(NullReferenceException)
            {
                return null;
            }
        }

        public string Id { get; }
    }
}