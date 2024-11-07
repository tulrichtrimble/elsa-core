using Elsa.MassTransit.Messages;
using FastEndpoints;
using MassTransit;

namespace Elsa.Samples.AspNet.MassTransitWorkflow.Endpoints.Messages.Create;

public class Create : EndpointWithoutRequest
{
    private readonly IBus _bus;

    public Create(IBus bus)
    {
        _bus = bus;
    }
    
    public override void Configure()
    {
        Post("messages");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await _bus.Publish<DataPropertyMessage>(new DataPropertyMessage
        {
            Key = "Hello World from the bus.",
            Data = new Dictionary<string, string>()
            {
                {"key", DateTimeOffset.Now.ToString() }
            }
        }, cancellationToken);
    }
}