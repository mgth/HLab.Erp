using System.ComponentModel;
using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Views
{
    public interface IDefaultView
    {
        object Icon { get; }
        string Caption { get; }
    }
    
    /// <summary>
    /// Logique d'interaction pour DefaultView.xaml
    /// </summary>
    public partial class DefaultView : UserControl
        //,IView<ViewModeDefault,INotifyPropertyChanged>
    {
        public DefaultView()
        {
            InitializeComponent();
        }
    }
}
