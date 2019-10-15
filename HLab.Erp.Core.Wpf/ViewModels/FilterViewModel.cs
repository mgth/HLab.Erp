using System.ComponentModel;
using System.Linq;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ViewModels
{
    public interface IFilterViewModel<T> : INotifyPropertyChanged
        where T : class, IEntity, new()
    {
        IQueryable<T> Result { get; }
    }
}
