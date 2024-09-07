using System.Windows.Controls;
using HLab.Erp.Workflows.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Workflows
{
    /// <summary>
    /// Logique d'interaction pour WorkFlowState.xaml
    /// </summary>
    public partial class WorkFlowStageView : UserControl, IView<WorkflowViewModel>
    {
        public WorkFlowStageView()
        {
            InitializeComponent();
        }

    }
}
