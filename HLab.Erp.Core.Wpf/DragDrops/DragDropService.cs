using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Core.DragDrops
{
    [Export(typeof(IDragDropService)), Singleton]
    public class DragDropService : IDragDropService
    {
        [Import] private readonly Func<Panel, FrameworkElement, bool, ErpDragDrop> _dragDropGetter;
        private readonly Dictionary<string, Panel> _canvas = new Dictionary<string, Panel>();

        public Panel GetDragCanvas(string name="") => _canvas[name];

        public void RegisterDragCanvas(Panel canvas, string name) => _canvas.Add(name, canvas);

        public ErpDragDrop Get(FrameworkElement source, bool send)
            => _dragDropGetter(this.GetDragCanvas() as Panel, source, send);
    }
}