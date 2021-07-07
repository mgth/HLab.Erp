using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ViewModels.EntityLists
{
    public interface IObjectMapper : INotifyPropertyChanged
    {
        int Id { get; }
        object Model { get; }
        public bool IsSelected { get; set; }
        void Refresh(string column);
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
            result = _columns.GetValue(Model, binder.Name);
            return true;
        }

        public void Refresh(string column) => OnPropertyChanged(column);

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}