using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

//using AvalonDock.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Core.EntityLists;
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

        private void Sv_LayoutUpdated(object sender, System.EventArgs e)
        {
            //AdjustColumns();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is IEntityListViewModel vm)
                vm.SelectedIds = ListView.SelectedItems.OfType<IObjectMapper>().Select(o => o.Id).ToList();
        }

        private void OnSelectIdsChange()
        {
            if (DataContext is IEntityListViewModel vm)
            {
                var ids = ListView.SelectedItems.OfType<IObjectMapper>().Select(o => o.Id).ToList();
                var vmIds = vm.SelectedIds.ToList();

                foreach (var id in vmIds)
                {
                    if(!ids.Contains(id)) ListView.SelectedItems.Add(ListView.Items.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
                }

                foreach (var id in ids)
                {
                    if(!vmIds.Contains(id)) ListView.SelectedItems.Remove(ListView.Items.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
                }
            }
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

                var sv = ListView.FindVisualChildren<ScrollViewer>().FirstOrDefault();

                if (sv != null)
                {
                    sv.LayoutUpdated += Sv_LayoutUpdated; ;
                }

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
                vm.PropertyChanged += Vm_PropertyChanged;
            }
            if(e.OldValue is IEntityListViewModel oldVm)
            {
                oldVm.PropertyChanged -= Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedIds")
                OnSelectIdsChange();
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

        private void AdjustColumns()
        {
            var widths = new List<double>();
            if (DataContext is IEntityListViewModel vm)
            {
                foreach(var g in vm.Columns.Columns.Values)
                {
                    if(g.Hidden) continue;
                    widths.Add(g.Width);
                }
            } else return;


            var width = ListView.ActualWidth;
            if(ListView.View is GridView gridView)
            {
                var sv = ListView.FindVisualChildren<ScrollViewer>().FirstOrDefault();

                width = sv.ViewportWidth;

                //var widths = gridView.Columns.Select(c => c.Width).ToArray();
                var total = widths.Where(e => !double.IsNaN(e)).Sum();

                var diff = total - width;

                for(int i = 0; i <  gridView.Columns.Count; i++)
                {
                    var column = gridView.Columns[i];
                    column.Width = width * widths[i] / total;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AdjustColumns();
        }
    }
}
