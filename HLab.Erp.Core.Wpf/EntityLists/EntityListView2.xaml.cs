using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using AvalonDock.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView2 : UserControl, 
        IView<ViewModeDefault, IEntityListViewModel>,
        IViewClassDocument, IViewClassDefault
    {
        public EntityListView2()
        {
            InitializeComponent();
            DataContextChanged += EntityListView_DataContextChanged;
            Loaded += EntityListView2_Loaded;
        }

        private void EntityListView2_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var filter = this.FindVisualAncestor<FilterView>();
            if (filter != null)
            {
                DataGrid.MaxHeight = 200;
            }
        }

        private void EntityListView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEntityListViewModel vm)
            {
                vm.Populate(DataGrid);
            }
        }

        public string ContentId => nameof(EntityListView);

        private void DataGridColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e)
        {
                if (sender is DataGridColumnHeader header)
                {
//                    header.Tag
                }
        }
    }
}
