using System.Windows;
using System.Windows.Controls;

namespace HLab.Erp.Core.DragDrops
{
    public interface IDragDropService
    {
        Panel GetDragCanvas(string name = "");
        void RegisterDragCanvas(Panel canvas, string name ="");

        ErpDragDrop Get(FrameworkElement source, bool send);
    }
}
