using HLab.Erp.Base.Data;
using HLab.Mvvm.Application;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Base.Countries;

public class CountryListableViewModel : ViewModel<Country>, IListableModel
{
    public CountryListableViewModel() 
    { 
        _caption = this
            .WhenAnyValue(e => e.Model.Name)
            .ToProperty(this, e => e.Caption);

        _iconPath = this
            .WhenAnyValue(e => e.Model.IconPath)
            .ToProperty(this, e => e.IconPath);
    }

    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;

    public string IconPath => _iconPath.Value;
    readonly ObservableAsPropertyHelper<string> _iconPath;
}