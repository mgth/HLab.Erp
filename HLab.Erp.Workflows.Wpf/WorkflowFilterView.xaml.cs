using System.Windows.Controls;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Erp.Workflows.Interfaces;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Workflows
{
    /// <summary>
    /// Logique d'interaction pour FilterEntityView.xaml
    /// </summary>
    public partial class WorkflowFilterView : UserControl , IView<DefaultViewMode, IWorkflowFilter>, IFilterContentViewClass
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
