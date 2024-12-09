//using System.Dynamic;
//using System.Text.Json;
//using Elsa.JavaScript.Models;
//using Elsa.Kafka.Activities;
//using Elsa.Workflows;
//using Elsa.Workflows.Activities;
//using Trimble.Elsa.Activities.Kafka;

//namespace Elsa.Server.Web.Workflows;

//public class ConsumerWorkflow : WorkflowBase
//{
//    protected override void Build(IWorkflowBuilder builder)
//    {
//        var message = builder.WithVariable<AvroDataPropertyMessage>();

//        builder.WithVariable("message-var#name", "message-val");

//        builder.Name = "Consumer Workflow";
//        builder.Root = new Sequence
//        {
//            Activities =
//            {
//                new MessageReceived
//                {
//                    ConsumerDefinitionId = new("trimble-avro-consumer"),
//                    Topics = new(["elsa-test"]),
//                    Predicate = new(JavaScriptExpression.Create("getVariable(\"message\").Data.CapabilityName === getVariable(\"message-var#name\")")),
//                    Result = new(message),
//                    CanStartWorkflow = true
//                },
//                new WriteLine(c => JsonSerializer.Serialize(message.Get(c)))
//            }
//        };
//    }
//}