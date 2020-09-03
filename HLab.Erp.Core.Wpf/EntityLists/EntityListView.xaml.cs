using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView : UserControl, 
        IView<ViewModeDefault, IEntityListViewModel>,
        IViewClassDocument, IViewClassDefault
    {
        public EntityListView()
        {
            InitializeComponent();
            DataContextChanged += EntityListView_DataContextChanged;
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
