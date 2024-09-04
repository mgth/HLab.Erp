using ReactiveUI;
using System;

namespace HLab.Erp.Data;

public class ForeignProperty<T> 
    where T : IEntity<int>
{
    public ForeignProperty(ObservableAsPropertyHelper<T> foreign, Action<int?> idSetter)
    {
        Foreign = foreign;
    }
    public int? Id { get; }
    public ObservableAsPropertyHelper<T> Foreign { get; }
}