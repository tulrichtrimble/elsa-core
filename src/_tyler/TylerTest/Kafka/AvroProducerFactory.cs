using Confluent.SchemaRegistry;
using Elsa.Kafka;
using Elsa.Kafka.Implementations;

namespace Trimble.Elsa.Activities.Kafka;

public class AvroProducerFactory<TK, TV> : IProducerFactory 
{
    public IProducer CreateProducer(CreateProducerContext workerContext)
    {
        var producerDefinition = workerContext.ProducerDefinition;

        CachedSchemaRegistryClient schemaRegistryClient = new(new SchemaRegistryConfig { Url = producerDefinition.SchemaRegistryUrl });
        var producer = new ProducerBuilderWithSerialization<TK, TV>(producerDefinition.Config)
           .SetKeyValueSerializers(schemaRegistryClient)
           .Build();
        ProducerProxy proxy = new ProducerProxy(producer);

        return proxy;
    }
}
