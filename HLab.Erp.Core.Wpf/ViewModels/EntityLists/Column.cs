using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core.ViewModels
{
    public class Column<T>
    {
        public Column(object caption, Func<T, object> getter,Expression<Func<T, object>> orderBy, string id, bool hidden)
        {
            Id = id ?? ("C" + Guid.NewGuid().ToString().Replace('-', '_'));
            Caption = caption;
            OrderBy = orderBy;
            _getter = getter;
            Hidden = hidden;
        }

        public bool Hidden { get; }
        public object Caption { get; }
        public Expression<Func<T, object>> OrderBy { get; }
        private readonly Func<T, object> _getter;

        public object Get(T value)
        {
            return _getter(value);
        }

        public string Id { get; }
    }
}