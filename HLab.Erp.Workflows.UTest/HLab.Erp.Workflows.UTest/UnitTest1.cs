using System;
using System.Linq;
using Xunit;

namespace HLab.Erp.Workflows.UTest
{
    class WorkflowHolder
    {
        public WorkflowHolder()
        {
            Workflow = new TestWorkflow(this);
        }

        public string Value { get; set; }
        public TestWorkflow Workflow { get; }
    }


    class TestWorkflow : Workflow<TestWorkflow,WorkflowHolder>
    {

        public static WorkflowState<TestWorkflow> FirstState = WorkflowState<TestWorkflow>.Create(c => c);

        public static WorkflowAction<TestWorkflow> ToSecondState = WorkflowAction<TestWorkflow>.Create(
            c => c.FromState(() => FirstState).ToState(() => SecondState));

        public static WorkflowAction<TestWorkflow> ToThirdState = WorkflowAction<TestWorkflow>.Create(
            c => c.FromState(()=>SecondState).ToState(()=>ThirdState));

        public static WorkflowAction<TestWorkflow> DirectToThirdState = WorkflowAction<TestWorkflow>.Create(
            c => c.FromState(()=>FirstState).ToState(()=>ThirdState));

        public static WorkflowState<TestWorkflow> SecondState = WorkflowState<TestWorkflow>.Create(
            c => c
            .AllowedWhen(w => w.Target.Value == "Ok")
            .WithMessage(w=>"Wrong target value :" + w.Target.Value));


        public static WorkflowState<TestWorkflow> ThirdState = WorkflowState<TestWorkflow>.Create(
            c => c.AllowedWhen(w => w.Target.Value == "Ok").WithMessage(w => "Wrong target value :" + w.Target.Value));


        public TestWorkflow(WorkflowHolder target) : base(target)
        {
        }
    }


    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var obj = new WorkflowHolder();
            var result = false;

            var actions = obj.Workflow.GetActions();
            Assert.False(actions.Any());

            result = obj.Workflow.SetState(()=>TestWorkflow.FirstState);
            Assert.True(result);

            result = obj.Workflow.SetState(()=>TestWorkflow.SecondState);
            Assert.False(result);

            obj.Value = "Ok";

            actions = obj.Workflow.GetActions().ToList();
            Assert.True(actions.Any());

            result = obj.Workflow.SetState(()=>TestWorkflow.ThirdState);
            Assert.True(result);

        }
    }
}
