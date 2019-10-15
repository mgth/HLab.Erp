using System.ComponentModel;

namespace HLab.Erp.Data
{
    public interface ILineEntityViewModel<T, TParent> : ILineEntityViewModel<T>
        where T : class, INotifyPropertyChanged, IEntity, new()
        where TParent : class, IListEntityViewModel
    {
        TParent ParentList { get; set; }
        //ViewModel<T, TContext> ViewModel { get; set; }
    }


}
