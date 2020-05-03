using System.Dynamic;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ViewModels.EntityLists
{
    public sealed class ObjectMapper<T> : DynamicObject
        where T : class, IEntity
    {
        private readonly T _model;
        private readonly IColumnsProvider<T> _columns;

        public T Model => _model;

        public ObjectMapper(T model, IColumnsProvider<T> columns)
        {
            _model = model;
            _columns = columns;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch (binder.Name)
            {
                //case "Model":
                //    result = _model;
                //    return true;
                default:
                    result = _columns.GetValue(_model, binder.Name);
                    return true;
            }
        }
    }
}