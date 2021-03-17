using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Workflows
{
    /// <summary>
    /// Logique d'interaction pour FilterEntityView.xaml
    /// </summary>
    public partial class WorkflowFilterView : UserControl , IView<ViewModeDefault, IWorkflowFilter>, IFilterContentViewClass
    {
        public WorkflowFilterView()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
        }
    }
}
