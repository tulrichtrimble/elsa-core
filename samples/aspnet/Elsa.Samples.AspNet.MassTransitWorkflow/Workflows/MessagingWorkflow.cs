using Elsa.MassTransit.Activities;
using Elsa.MassTransit.Messages;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Helpers;

namespace Elsa.Samples.AspNet.MassTransitWorkflow.Workflows;

public class MessagingWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var busMessage = builder.WithVariable<DataPropertyMessage>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new MessageReceived
                {
                    CanStartWorkflow = true,
                    Type = ActivityTypeNameHelper.GenerateTypeName(typeof(DataPropertyMessage)),
                    MessageType = typeof(DataPropertyMessage),
                    Result = new (busMessage)
                },
                new WriteLine(context => $"Received message from Bus: {busMessage.Get(context)?.Key}")
            }
        };
    }
}