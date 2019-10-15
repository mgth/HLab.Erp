using HLab.DependencyInjection.Annotations;


namespace HLab.Erp.Core.Wpf.DragDrops
{
    [Export(typeof(IDragDropService)), Singleton]
    public class DragDropServiceWpf : DragDropService
    {
    }
}
