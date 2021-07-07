using System.IO;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;
using Microsoft.Win32;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    using H = H<CountryViewModel>;

    public class CountryViewModel : EntityViewModel<Country>
    {
        public CountryViewModel() => H.Initialize(this);

        public override object Header => _header.Get();

        private readonly IProperty<object> _header = H.Property<object>(c => c
            .On(e => e.Model.Name)
            .Set(e => (object)("{Country} - " + e.Model.Name))
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
