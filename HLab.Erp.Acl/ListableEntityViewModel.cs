using System.ComponentModel;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Acl;

public interface IListableEntityViewModel
{
    object Header { get; }
    string IconPath { get; }
}


public class ListableEntityViewModel<T> : EntityViewModel<T>, IListableEntityViewModel
    where T : class, IEntity<int>, INotifyPropertyChanged, IListableModel
{
    protected ListableEntityViewModel():base(null){}

    public ListableEntityViewModel(Injector i) : base(i) 
    {
        _header = this.WhenAnyValue(
                e => e.EntityName, 
                e => e.Model.Caption, 
                selector: (n, c) => $"{n}\n{c}")
            .ToProperty(this, nameof(Header));

        _iconPath = this.WhenAnyValue(e => e.Model.IconPath)
            .ToProperty(this, nameof(IconPath));
    }

    public override object Header => _header.Value;
    readonly ObservableAsPropertyHelper<string> _header;

    public override string IconPath => _iconPath.Value;

    readonly ObservableAsPropertyHelper<string> _iconPath;

}