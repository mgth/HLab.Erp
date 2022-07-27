using System.ComponentModel;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.EntitySelectors
{
    public class ForeignViewModel<T> : EntityViewModel<T>, IForeignViewModel
        where T : class, IEntity<int>, INotifyPropertyChanged
    {

         public ForeignViewModel(Injector i) : base(i)
         {
             H<ForeignViewModel<T>>.Initialize(this);
         }

         public ICommand OpenCommand { get; } = H<ForeignViewModel<T>>.Command(c => c.Action(
            e => e.Injected.Docs?.OpenDocumentAsync(e.Model))
        );
        public ICommand SelectCommand { get; } = H<ForeignViewModel<T>>.Command(c => c);
    }
}