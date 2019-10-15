using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace HLab.Erp.Core.ApplicationServices
{
    /// <summary>
    /// Interface definition for parent viewmodel of <seealso cref="AvalonDockLayoutViewModel"/>
    /// class. This interface defines the properties and methods required to be available for access
    /// by the layout load/save viewmodel class.
    /// </summary>
    public interface ILayoutViewModelParent
    {
        bool IsBusy { get; set; }
        string DirAppData { get; }

        void ReloadContentOnStartUp(LayoutSerializationCallbackEventArgs args);
    }
}
