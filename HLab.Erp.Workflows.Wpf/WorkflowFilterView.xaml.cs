using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Workflows
{
    /// <summary>
    /// Logique d'interaction pour FilterEntityView.xaml
    /// </summary>
    public partial class WorkflowFilterView : UserControl , IView<ViewModeDefault, IWorkflowFilterViewModel>, IFilterContentViewClass
    {
        public WorkflowFilterView()
        {
            InitializeComponent();
        }
    }
}
