using System;
using System.Runtime.CompilerServices;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Data
{
    public class ForeignProperty<T> : IForeign<T>
        where T:IEntity<int>
    {
        public ForeignProperty(IProperty<int?> id, IProperty<T> value)
        {
            Id = id;
            Value = value;
        }

        public IProperty<int?> Id { get; }
#if DEBUG
        public T Get([CallerMemberName]string name = null) => Value.Get(name);
#else        
        public T Get() => Value.Get();
#endif
        public void Set(T value) => Id.Set(value?.Id);

        public IProperty<T> Value { get; }


        public void Dispose()
        {
        }

        public INotifyPropertyChangedWithHelper Parent
        {
            get => Id.Parent;
            set
            {
                Id.Parent = value;
                Value.Parent = value;
            }
        }
        public void OnDispose(Action action)
        {
        }
    }
}