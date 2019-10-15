using HLab.Mvvm.Annotations;
using UserControl = System.Windows.Controls.UserControl;

namespace HLab.Erp.Core.WebService
{
    /// <summary>
    /// Logique d'interaction pour BrowserControl.xaml
    /// </summary>
    public partial class BrowserView : UserControl, IViewClassAnchorable
        ,IView<ViewModeDefault,BrowserViewModel>
    {
        public BrowserView()
        {
            InitializeComponent();
        }
        public string ContentId => GetType().Name;
    }
}
