using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

//using AvalonDock.Controls;
using HLab.Base.Wpf.Extensions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView2 : UserControl
        //,IView<DefaultViewMode, IEntityListViewModel>,
        //IDocumentViewClass, IDefaultViewClass
    {
        public EntityListView2()
        {
            InitializeComponent();
            DataContextChanged += EntityListView_DataContextChanged;
            Loaded += EntityListView2_Loaded;
            ListView.SelectionChanged += ListView_SelectionChanged;
            Unloaded += EntityListView2_Unloaded;

        }

        void Sv_LayoutUpdated(object sender, System.EventArgs e)
        {
            //AdjustColumns();
        }

        void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is IEntityListViewModel vm)
                vm.SelectedIds = ListView.SelectedItems.OfType<IObjectMapper>().Select(o => o.Id).ToList();
        }

        void OnSelectIdsChange()
        {
            if (DataContext is IEntityListViewModel vm)
            {
                var ids = ListView.SelectedItems.OfType<IObjectMapper>().Select(o => o.Id).ToList();
                var vmIds = vm.SelectedIds.ToList();

                foreach (var id in vmIds)
                {
                    if (!ids.Contains(id)) ListView.SelectedItems.Add(ListView.Items.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
                }

                foreach (var id in ids)
                {
                    if (!vmIds.Contains(id)) ListView.SelectedItems.Remove(ListView.Items.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
                }
            }
        }

        void EntityListView2_Loaded(object sender, RoutedEventArgs e)
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


        void EntityListView2_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IEntityListViewModel vm)
            {
                vm.Stop();
            }
        }

        void EntityListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IEntityListViewModel vm)
            {
                vm.Populate(ListView);
                vm.PropertyChanged += Vm_PropertyChanged;
            }
            if (e.OldValue is IEntityListViewModel oldVm)
            {
                oldVm.PropertyChanged -= Vm_PropertyChanged;
            }
        }

        void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedIds")
                OnSelectIdsChange();
        }





        public string ContentId => nameof(EntityListView);

        void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not IEntityListViewModel vm) return;
            if (sender is not ListViewItem { Content: IObjectMapper mapper }) return;
            var entity = mapper.Model;
            if (entity == null) return;

            if (vm.OpenCommand.CanExecute(entity))
            {
                vm.OpenCommand.Execute(entity);
            }
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.SwitchVisibility();
        }

        void AdjustColumns()
        {
            var widths = new List<double>();
            if (DataContext is IEntityListViewModel vm)
            {
                foreach (var g in vm.Columns.Columns.Values)
                {
                    widths.Add(g.Width);
                }
            }
            else return;


            var width = ListView.ActualWidth;
            if (ListView.View is GridView gridView)
            {
                var sv = ListView.FindVisualChildren<ScrollViewer>().FirstOrDefault();

                width = sv.ViewportWidth;

                //var widths = gridView.Columns.Select(c => c.Width).ToArray();
                var total = widths.Where(e => !double.IsNaN(e)).Sum();

                var diff = total - width;

                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    var column = gridView.Columns[i];
                    column.Width = width * widths[i] / total;
                }
            }
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            AdjustColumns();
        }

        void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not IEntityListViewModel { AllowManualOrder: true }) return;

            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (sender is not ListViewItem draggedItem) return;

            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);

            draggedItem.IsSelected = true;
        }

        void SetDragTag(FrameworkElement fe, string tag)
        {
            foreach (var e in ListView.FindVisualChildren<ListViewItem>())
            {
                e.Tag = (e == fe) ? tag : null;
            }
        }

        void ListView_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Drop");

            if (sender is not FrameworkElement fe) return;
            
            SetDragTag(fe,null);

            if (DataContext is not IEntityListViewModel vm) return;

            if (e.Source is not FrameworkElement { DataContext: IObjectMapper to }) return;

            var formats = e.Data.GetFormats();
            if (formats.Length < 1) return;

            var data = e.Data.GetData(formats[0]);
            if (data is not IObjectMapper from) return;

            var after = false;
            if (sender is IInputElement ie)
            {
                var pos = e.GetPosition(ie);
                if (pos.Y > fe.ActualHeight / 2) after = true;
            }

            vm.Drop(from.Model, to.Model, after);

            ListView.SelectedItem = to;
        }

        void ListView_DragOver(object sender, DragEventArgs e)
        {

            if (sender is not ListViewItem fe)
            {
                Debug.WriteLine(sender.GetType());
                return;
            }

            var pos = e.GetPosition(fe);
            var after = (pos.Y > fe.ActualHeight / 2);

            SetDragTag(fe,after?"InsertDown":"InsertUp");
        }

        void ListView_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            //fe.Tag = null;
        }

    }
}
