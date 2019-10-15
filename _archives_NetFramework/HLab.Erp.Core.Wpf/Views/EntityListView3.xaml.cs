using System.Windows.Controls;
using HLab.Erp.Core.Wpf.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView3 : UserControl, 
        IView<ViewModeDefault, IListViewModel>,
        IViewClassDocument

    {
        public EntityListView3()
        {
            InitializeComponent();
            DataContextChanged += EntityListView3_DataContextChanged;
        }

        private void EntityListView3_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IListViewModel vm)
            {
                vm.PopulateDataGrid(DataGrid);
            }
        }

        public string ContentId => nameof(EntityListView3);
    }
}
