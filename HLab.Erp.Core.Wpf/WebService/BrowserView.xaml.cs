using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using UserControl = System.Windows.Controls.UserControl;

namespace HLab.Erp.Core.Wpf.WebService
{
    /// <summary>
    /// Logique d'interaction pour BrowserControl.xaml
    /// </summary>
    public partial class BrowserView : UserControl, IAnchorableViewClass
        ,IView<DefaultViewMode,BrowserViewModel>
    {
        public BrowserView()
        {
            InitializeComponent();
        }
        public string ContentId => GetType().Name;
    }
}
