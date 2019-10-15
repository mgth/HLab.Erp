using System.Windows.Controls;

namespace HLab.Erp.Core.Views
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
