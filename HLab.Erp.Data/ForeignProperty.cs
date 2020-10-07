using System.Runtime.CompilerServices;
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

        public void SetParent(object parent, INotifyClassHelper parser)
        {
            Id.SetParent(parent,parser);
            Value.SetParent(parent,parser);
        }
    }
}