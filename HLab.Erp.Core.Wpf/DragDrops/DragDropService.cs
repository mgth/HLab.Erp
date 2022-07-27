using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core.DragDrops;

namespace HLab.Erp.Core.Wpf.DragDrops
{
    public class DragDropService : IDragDropService
    {
        readonly Func<Panel, FrameworkElement, bool, ErpDragDrop> _dragDropGetter;
        readonly Dictionary<string, Panel> _canvas = new();

        public DragDropService(Func<Panel, FrameworkElement, bool, ErpDragDrop> dragDropGetter)
        {
            _dragDropGetter = dragDropGetter;
        }

        public Panel GetDragCanvas(string name="") => _canvas[name];

        public void RegisterDragCanvas(Panel canvas, string name) => _canvas.Add(name, canvas);

        public ErpDragDrop Get(FrameworkElement source, bool send)
            => _dragDropGetter(this.GetDragCanvas() as Panel, source, send);
    }
}