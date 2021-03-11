using System.ComponentModel;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.EntitySelectors
{
    public class ForeignViewModel<T> : EntityViewModel<T>, IForeignViewModel
        where T : class, IEntity<int>, INotifyPropertyChanged
    {
        public ForeignViewModel() => H<ForeignViewModel<T>>.Initialize(this);

        [Import] private IDocumentService _doc;
        public ICommand OpenCommand { get; } = H<ForeignViewModel<T>>.Command(c => c.Action(
            e => e._doc.OpenDocumentAsync(e.Model) )
        );
        public ICommand SelectCommand { get; } = H<ForeignViewModel<T>>.Command(c => c);
    }
}