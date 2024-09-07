using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;
using Microsoft.Xaml.Behaviors.Layout;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using HLab.Mvvm.Wpf.Views;
using HLab.Erp.Workflows.Interfaces;
using HLab.Base.Wpf.Controls;

namespace HLab.Erp.Workflows;

public static class HighlightHelper
{
    public static void SetHighlights<T>(this IView<T> view, Func<T, IWorkflow> w)
    {
        if (view is FrameworkElement fe)
        {
            fe.DataContextChanged += (o, e) =>
            {
                if (!view.TryGetViewModel(out var vm)) return;

                var workflow = w(vm);

                if (workflow.Highlights is INotifyCollectionChanged c)
                {
                    c.CollectionChanged += (target, arg) =>
                    {
                        switch (arg.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                if (arg.NewItems != null)
                                    foreach (var item in arg.NewItems)
                                    {
                                        if (item is string s)
                                            Highlight(view, s);
                                    }

                                break;
                            case NotifyCollectionChangedAction.Reset:
                                Highlight(view, null);
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                break;
                            case NotifyCollectionChangedAction.Move:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    };
                }

            };
        }
    }


    static void Highlight(object e, string name)
    {
        if (e == null) return;

        switch (e)
        {
            case IMandatoryNotFilled fw:
                HighlightUI(fw as UIElement, name);
                break;

            case TextBox tb:
                HighlightUI(tb, name);
                break;

            case Panel p:
                foreach (var c in p.Children)
                {
                    Highlight(c, name);
                }
                break;

            case ContentControl contentControl:
                Highlight(contentControl.Content, name);
                break;
            case Popup popup:
                HighlightUI(popup.Child, name);
                break;
            case ComboBox:
            case ListView:
            case TextBlock:
            case Shape:
            case Decorator:
            case Calendar:
                break;
            default:
                break;

        }
    }

    static void HighlightUI(UIElement ui, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            RemoveHighlightUi(ui);
            return;
        }

        var binding = BindingOperations.GetBinding(ui, BindingProperty(ui));
        if (binding == null) return;

        var bName = binding.Path.Path.Split('.').Last();
        if (bName != name) return;

        var al = AdornerLayer.GetAdornerLayer(ui);
        var c = new AdornerContainer(ui)
        {
            IsHitTestVisible = false,
            Child = new MandatoryAdorner()
        };
        al?.Add(c);
    }

    static void RemoveHighlightUi(UIElement ct)
    {
        var al = AdornerLayer.GetAdornerLayer(ct);
        var ads = al?.GetAdorners(ct);
        if (ads == null) return;
        foreach (var ad in ads)
        {
            if (ad is not AdornerContainer ac) continue;
            if (ac.Child is MandatoryAdorner) al.Remove(ac);
        }
    }


    static DependencyProperty BindingProperty(UIElement ui)
    {
        return ui switch
        {
            NumTextBox => NumTextBox.ValueProperty,
            TextBox => TextBox.TextProperty,
            IMandatoryNotFilled mnf => mnf.MandatoryProperty,
            Selector => Selector.SelectedValueProperty,
            _ => null
        };
    }

}