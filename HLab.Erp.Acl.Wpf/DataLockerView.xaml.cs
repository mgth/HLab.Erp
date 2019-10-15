using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl
{
    /// <summary>
    /// Logique d'interaction pour DataLockerView.xaml
    /// </summary>
    public partial class DataLockerView : UserControl, IView<DataLocker>
    {
        public DataLockerView()
        {
            InitializeComponent();
        }
    }
}
