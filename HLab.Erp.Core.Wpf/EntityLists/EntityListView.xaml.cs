using System.Windows.Controls;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm.Annotations;

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
                vm.PopulateDataGrid(DataGrid);
            }
        }

        public string ContentId => nameof(EntityListView);
    }
}
