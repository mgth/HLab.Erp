using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using HLab.Erp.Core.Avalonia.ListFilters;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Core.Avalonia.EntityLists;

/// <summary>
/// Logique d'interaction pour EntityListView.axaml
/// </summary>
public partial class EntityListView : UserControl,
    IView<DefaultViewMode, IEntityListViewModel>,
    IDocumentViewClass, IDefaultViewClass
{
    public EntityListView()
    {
        InitializeComponent();
        DataContextChanged += EntityListView_DataContextChanged;
        Loaded += EntityListView_Loaded;
        ListView.SelectionChanged += ListView_SelectionChanged;
        ListView.DoubleTapped += ListView_DoubleTapped;
        Unloaded += EntityListView_Unloaded;
    }

    void ListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
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
                if (!ids.Contains(id)) ListView.SelectedItems.Add(ListView.ItemsSource.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
            }

            foreach (var id in ids)
            {
                if (!vmIds.Contains(id)) ListView.SelectedItems.Remove(ListView.ItemsSource.OfType<IObjectMapper>().FirstOrDefault(i => i.Id == id));
            }
        }
    }

    void EntityListView_Loaded(object? sender, RoutedEventArgs e)
    {
        var filter = this.GetVisualAncestors().OfType<FilterView>().FirstOrDefault();
        if (filter != null)
        {
            ListView.MaxHeight = 200;
        }
        if (DataContext is IEntityListViewModel vm)
        {
            vm.Start();
        }
    }

    void EntityListView_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IEntityListViewModel vm)
        {
            vm.Stop();
        }
    }

    void EntityListView_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (_oldVm != null)
        {
            _oldVm.PropertyChanged -= Vm_PropertyChanged;
            _oldVm = null;
        }

        if (DataContext is IEntityListViewModel vm)
        {
            vm.Populate(ListView);
            vm.PropertyChanged += Vm_PropertyChanged;
            _oldVm = vm;
        }
    }

    IEntityListViewModel? _oldVm;

    void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedIds")
            OnSelectIdsChange();
    }

    public string ContentId => nameof(EntityListView);

    void ListView_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not IEntityListViewModel vm) return;
        if (e.Source is not Visual source) return;

        var row = source.GetVisualAncestors().OfType<DataGridRow>().FirstOrDefault()
            ?? source as DataGridRow;
        if (row is not { DataContext: IObjectMapper mapper }) return;

        var entity = mapper.Model;
        if (entity == null) return;

        if (vm.OpenCommand.CanExecute(entity))
        {
            vm.OpenCommand.Execute(entity);
        }
    }

    void ButtonBase_OnClick(object? sender, RoutedEventArgs e)
    {
        MessageTextBlock.IsVisible = !MessageTextBlock.IsVisible;
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

        var width = ListView.Bounds.Width;

        var total = widths.Where(w => !double.IsNaN(w)).Sum();
        if (total <= 0 || width <= 0) return;

        for (var i = 0; i < ListView.Columns.Count && i < widths.Count; i++)
        {
            var column = ListView.Columns[i];
            if (double.IsNaN(widths[i])) continue;
            column.Width = new DataGridLength(width * widths[i] / total);
        }
    }

    void Button_Click(object? sender, RoutedEventArgs e)
    {
        AdjustColumns();
    }
}
