using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Core.Annotations;
using HLab.Erp.Core.WebService;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;
using ReactiveUI;


namespace HLab.Erp.Core.Wpf.WebService;

public class BrowserServiceModule(IDocumentService docs, IMenuService menu) : ReactiveModel, IBootloader
{
    public ICommand OpenDocumentCommand { get; } = ReactiveCommand.Create(() => docs.OpenDocumentAsync(typeof(IBrowserService)));

    public Task LoadAsync(IBootContext bootstrapper)
    {
        menu.RegisterMenu("tools/internet", "{Internet}", OpenDocumentCommand, "icons/internet");
        return Task.CompletedTask;
    }
}