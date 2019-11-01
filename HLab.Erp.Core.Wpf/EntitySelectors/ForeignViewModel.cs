using System.ComponentModel;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntitySelectors
{
    public class ForeignViewModel<T> : EntityViewModel<ForeignViewModel<T>,T>, IForeignViewModel
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        public ICommand OpenCommand { get; } = H.Command(c => c);
        public ICommand SelectCommand { get; } = H.Command(c => c);
    }
}