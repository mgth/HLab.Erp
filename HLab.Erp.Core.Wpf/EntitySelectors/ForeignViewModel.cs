using System.ComponentModel;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.EntitySelectors
{
    public class ForeignViewModel<T> : EntityViewModel<ForeignViewModel<T>,T>, IForeignViewModel
        where T : class, IEntity<int>, INotifyPropertyChanged
    {

        [Import] private IDocumentService _doc;
        public ICommand OpenCommand { get; } = H.Command(c => c.Action(
            e => e._doc.OpenDocument(e.Model) )
        );
        public ICommand SelectCommand { get; } = H.Command(c => c);
    }
}