using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    /// <summary>
    /// Logique d'interaction pour CountryView.xaml
    /// </summary>
    public partial class CountryToolsView : UserControl, IView<CountryToolsViewModel>, IViewClassDocument
    {
        public CountryToolsView()
        {
            InitializeComponent();
        }
    }
}
