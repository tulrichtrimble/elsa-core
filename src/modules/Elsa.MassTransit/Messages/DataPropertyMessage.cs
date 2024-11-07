namespace Elsa.MassTransit.Messages;
public record DataPropertyMessage
{
    public string Key { get; init; }
    public Dictionary<string, string> Data { get; init; }
}