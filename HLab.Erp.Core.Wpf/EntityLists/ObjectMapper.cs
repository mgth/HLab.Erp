using System.Dynamic;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ViewModels.EntityLists
{
    public interface IObjectMapper
    {
        int Id { get; }
        object Model { get; }
        public bool IsSelected { get; set; }
    }

    public sealed class ObjectMapper<T> : DynamicObject, IObjectMapper
        where T : class, IEntity
    {
        private readonly IColumnsProvider<T> _columns;

        public int Id
        {
            get
            {
                if (Model is IEntity<int> e) return e.Id; 
                return -1;
            }
        }

        object IObjectMapper.Model => Model;
        public T Model { get; }

        public bool IsSelected { get; set; }


        public ObjectMapper(T model, IColumnsProvider<T> columns)
        {
            Model = model;
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
                default:
                    result = _columns.GetValue(Model, binder.Name);
                    return true;
            }
        }
    }
}