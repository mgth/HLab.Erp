using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    /// <summary>
    /// Logique d'interaction pour CountryView.xaml
    /// </summary>
    public partial class CountryView : UserControl, IView<CountryViewModel>, IViewClassDocument
    {
        public CountryView()
        {
            InitializeComponent();
        }
    }
}
