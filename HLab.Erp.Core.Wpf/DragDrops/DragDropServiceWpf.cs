using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Core.DragDrops
{
    [Export(typeof(IDragDropService)), Singleton]
    public class DragDropServiceWpf : DragDropService
    {
    }
}
