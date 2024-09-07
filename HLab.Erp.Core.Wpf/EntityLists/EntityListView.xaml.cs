using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView : UserControl 
        //,IView<DefaultViewMode, IEntityListViewModel>
        //,IDocumentViewClass, IDefaultViewClass
    {
        public EntityListView()
        {
            InitializeComponent();
            DataContextChanged += EntityListView_DataContextChanged;
        }

        void EntityListView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEntityListViewModel vm)
            {
                vm.Populate(DataGrid);
            }
        }

        public string ContentId => nameof(EntityListView);

        void DataGridColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e)
        {
                if (sender is DataGridColumnHeader header)
                {
//                    header.Tag
                }
        }
    }
}
