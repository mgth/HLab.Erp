using System.ComponentModel;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using ReactiveUI;

namespace HLab.Erp.Core.EntitySelectors;

public class ForeignViewModel<T> : EntityViewModel<T>, IForeignViewModel
    where T : class, IEntity<int>, INotifyPropertyChanged
{

    public ForeignViewModel(Injector i) : base(i)
    {
        OpenCommand = ReactiveCommand.Create(() => Injected.Docs?.OpenDocumentAsync(Model));
    }

    public ICommand OpenCommand { get; } 
    public ICommand SelectCommand { get; } 
}