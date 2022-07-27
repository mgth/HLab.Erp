using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core.DragDrops;

namespace HLab.Erp.Core.Wpf.DragDrops
{
    public class DragDropServiceWpf : DragDropService
    {
        public DragDropServiceWpf(Func<Panel, FrameworkElement, bool, ErpDragDrop> dragDropGetter) : base(dragDropGetter)
        {
        }
    }
}
