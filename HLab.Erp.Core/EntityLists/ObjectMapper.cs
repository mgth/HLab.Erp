#nullable enable
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists
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
        readonly IColumnsProvider<T> _columns;

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

            columns.RegisterTriggers(model, OnPropertyChanged);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return true;
        }

        readonly ConcurrentDictionary<string, object> _dict = new();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _dict.GetOrAdd(binder.Name, n => _columns.GetValue(Model, n));
            return true;
        }

        public void Refresh(string column) => OnPropertyChanged(column);

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _dict.TryRemove(propertyName, out var o);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}