using System.IO;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;
using Microsoft.Win32;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    using H = H<CountryViewModel>;

    public class CountryViewModel : ListableEntityViewModel<Country>
    {
        public CountryViewModel(Injector i):base(i) => H.Initialize(this);

        public ICommand PastIconCommand { get; } = H.Command(c => c
            .Action(e =>
                {
                    var openFileDialog = new OpenFileDialog {DefaultExt = "svg"};


                    if(openFileDialog.ShowDialog() == true)
                    {
                        var text = File.ReadAllText(openFileDialog.FileName);
                    }
                })
        );

    }
}
