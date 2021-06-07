using System.ComponentModel;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;


namespace HLab.Erp.Core.EntitySelectors
{
    public class ForeignViewModel<T> : EntityViewModel<T>, IForeignViewModel
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        private IDocumentService _doc;

        public void Inject(IDocumentService doc) => _doc = doc;

         public ForeignViewModel() => H<ForeignViewModel<T>>.Initialize(this);

         public ICommand OpenCommand { get; } = H<ForeignViewModel<T>>.Command(c => c.Action(
            e => e._doc?.OpenDocumentAsync(e.Model))
        );
        public ICommand SelectCommand { get; } = H<ForeignViewModel<T>>.Command(c => c);
    }
}