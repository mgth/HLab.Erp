using System.Windows.Controls;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Views
{
    /// <summary>
    /// Logique d'interaction pour ListCriterionView.xaml
    /// </summary>
    public partial class EntityListView : UserControl
        //,IView<ViewModeDefault, IListViewModel>
        , IView<ViewModeList, IListViewModel>
        ,IViewClassAnchorable
    {
        public EntityListView()
        {
            InitializeComponent();
        }

        public string ContentId => GetType().Name;
    }
}
