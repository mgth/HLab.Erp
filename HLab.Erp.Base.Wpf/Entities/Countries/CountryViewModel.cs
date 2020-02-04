using System.IO;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;
using Microsoft.Win32;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountryViewModel : EntityViewModel<CountryViewModel,Country>
    {
        public string Title => _title.Get();

        private readonly IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Name)
            .Set(e => "{Country} - " + e.Model.Name)
        );

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
