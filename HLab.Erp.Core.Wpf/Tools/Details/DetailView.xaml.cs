using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Tools.Details
{
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    public partial class DetailView : UserControl, IViewClassAnchorable, IView<ViewModeDefault, DetailsViewModel>
    {
        public DetailView()
        {
            InitializeComponent();
        }

        public string ContentId => GetType().Name;
    }
}
