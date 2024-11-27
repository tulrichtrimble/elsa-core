using Confluent.Kafka;
using Elsa.Common.Entities;
using Elsa.Kafka.Factories;

namespace Elsa.Kafka;

public class ProducerDefinition : Entity
{
    private Type factoryType;

    public string Name { get; set; } = default!;
    public Type FactoryType { get => factoryType; set => factoryType = value ?? typeof(DefaultProducerFactory); }
    public ProducerConfig Config { get; set; } = new();
    public string SchemaRegistryUrl { get; set; } = default!;
}