using Confluent.SchemaRegistry;
using Elsa.Kafka;
using Elsa.Kafka.Implementations;

namespace Trimble.Elsa.Activities.Kafka;
public class AvroConsumerFactory<TK, TV> : IConsumerFactory
{
    public IConsumer CreateConsumer(CreateConsumerContext workerContext)
    {
        var consumerDefinition = workerContext.ConsumerDefinition;

        CachedSchemaRegistryClient schemaRegistryClient = new(
            new SchemaRegistryConfig {
                //Url = consumerDefinition.SchemaRegistryUrl 
                Url = "http://localhost:8081"
            });

        var consumer = new ConsumerBuilderWithSerialization<TK, TV>(consumerDefinition.Config)
            .SetKeyValueDeserializers(schemaRegistryClient)
            .Build();

        return new ConsumerProxy(consumer);
    }
}
