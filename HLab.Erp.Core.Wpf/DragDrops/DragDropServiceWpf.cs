using System;
using System.Windows;
using System.Windows.Controls;
using Grace.DependencyInjection.Attributes;

namespace HLab.Erp.Core.DragDrops
{
    [Export(typeof(IDragDropService)), Singleton]
    public class DragDropServiceWpf : DragDropService
    {
        public DragDropServiceWpf(Func<Panel, FrameworkElement, bool, ErpDragDrop> dragDropGetter) : base(dragDropGetter)
        {
        }
    }
}
