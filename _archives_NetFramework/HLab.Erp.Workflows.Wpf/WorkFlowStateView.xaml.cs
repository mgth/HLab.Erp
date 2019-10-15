using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using H = HLab.Base.Wpf.DependencyHelper<Erp.Workflow.Wpf.WorkFlowStateView>;
namespace Erp.Workflow.Wpf
{
    /// <summary>
    /// Logique d'interaction pour WorkFlowState.xaml
    /// </summary>
    public partial class WorkFlowStateView : UserControl, IView<WorkflowViewModel>
    {
        public WorkFlowStateView()
        {
            InitializeComponent();
        }

    }
}
