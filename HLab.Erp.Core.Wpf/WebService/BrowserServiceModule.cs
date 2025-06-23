using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core.WebService;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;
using ReactiveUI;


namespace HLab.Erp.Core.Wpf.WebService;

public class BrowserServiceModule(IDocumentService docs, IMenuService menu) : Bootloader
{
    public ICommand OpenDocumentCommand { get; } = ReactiveCommand.Create(() => docs.OpenDocumentAsync(typeof(IBrowserService)));

    protected override BootState Load()
    {
        menu.RegisterMenu("tools/internet", "{Internet}", OpenDocumentCommand, "icons/internet");
        return BootState.Completed;
    }
}