using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Erp.Core.DragDrops
{
    public class DragDropService : IDragDropService
    {
        private Func<Panel, FrameworkElement, bool, ErpDragDrop> _dragDropGetter;
        private readonly Dictionary<string, Panel> _canvas = new Dictionary<string, Panel>();

        public void Inject(Func<Panel, FrameworkElement, bool, ErpDragDrop> dragDropGetter)
        {
            _dragDropGetter = dragDropGetter;
        }

        public Panel GetDragCanvas(string name="") => _canvas[name];

        public void RegisterDragCanvas(Panel canvas, string name) => _canvas.Add(name, canvas);

        public ErpDragDrop Get(FrameworkElement source, bool send)
            => _dragDropGetter(this.GetDragCanvas() as Panel, source, send);
    }
}