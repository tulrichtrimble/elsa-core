using Elsa.JavaScript.Models;
using Elsa.Kafka.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Memory;
using Elsa.Expressions.Models;

namespace Elsa.Server.Web.Workflows;

public class ProducerWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var messageContent = @"new Trimble.Elsa.Activities.Kafka.AvroDataPropertyMessage()
{
CorrelationId = ""correlation"",
Data = new Dictionary<string, string>()
    {
        { ""CapabilibtyName"", Variables.Get<string>(""message-var#name"") },
        { ""time"", DateTimeOffset.Now.ToString() }
    }
}";

        builder.Name = "Producer Workflow";
        builder.WithVariable("message-var#name", "message-val");
        builder.Root = new ProduceMessage
        {
            Topic = new("elsa-test"),
            ProducerDefinitionId = new("trimble-avro-producer"),
            Content = new(new Expression("CSharp", messageContent))
        };
    }
}