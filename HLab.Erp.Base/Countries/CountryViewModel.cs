using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using ReactiveUI;

namespace HLab.Erp.Base.Countries;

public class CountryViewModel(EntityViewModel<Country>.Injector i) : ListableEntityViewModel<Country>(i)
{
    public ICommand PastIconCommand { get; } = ReactiveCommand.Create(() =>
    {
        // TODO : Implement OpenDialog
        /*
            var openFileDialog = new OpenFileDialog { DefaultExt = "svg" };
            if (openFileDialog.ShowDialog() == true)
            {
                var text = File.ReadAllText(openFileDialog.FileName);
                // TODO : whats up next ?
            }
            */
    });
}