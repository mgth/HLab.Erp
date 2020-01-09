using System.Windows;
using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl
{
    /// <summary>
    /// Logique d'interaction pour DataLockerView.xaml
    /// </summary>
    public partial class DataLockerView : UserControl, IView<IDataLocker>
    {
        public DataLockerView()
        {
            InitializeComponent();
        }

        private void DataLockerView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is IDataLocker locker)) return;

            if (locker.SaveCommand.CanExecute(null))
                locker.SaveCommand.Execute(null);

            if (locker.CancelCommand.CanExecute(null))
                locker.CancelCommand.Execute(null);
        }
    }
}
