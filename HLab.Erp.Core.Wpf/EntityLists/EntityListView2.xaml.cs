using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AvalonDock.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Wpf.EntityLists
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
            ListView.SelectionChanged += ListView_SelectionChanged;
            Unloaded += EntityListView2_Unloaded;
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is IEntityListViewModel vm)
                vm.SelectedIds = ListView.SelectedItems.OfType<IObjectMapper>().Select(o => o.Id).ToList();
        }

        private void EntityListView2_Loaded(object sender, RoutedEventArgs e)
        {
            var filter = this.FindVisualAncestor<FilterView>();
            if (filter != null)
            {
                ListView.MaxHeight = 200;
            }
            if (DataContext is IEntityListViewModel vm)
            {
                vm.Start();
            }
        }
        private void EntityListView2_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IEntityListViewModel vm)
            {
                vm.Stop();
            }
        }

        private void EntityListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEntityListViewModel vm)
            {
                vm.Populate(ListView);
            }
        }

        public string ContentId => nameof(EntityListView);

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
                if (sender is DataGridColumnHeader header)
                {
//                    header.Tag
                }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not IEntityListViewModel vm) return;
            if (sender is not ListViewItem item) return;
            if (item.Content is not IObjectMapper mapper) return;
            var entity = mapper.Model;
            if (entity == null) return;

            if (vm.OpenCommand.CanExecute(entity))
            {
                vm.OpenCommand.Execute(entity);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.SwitchVisibility();
        }
    }
}
