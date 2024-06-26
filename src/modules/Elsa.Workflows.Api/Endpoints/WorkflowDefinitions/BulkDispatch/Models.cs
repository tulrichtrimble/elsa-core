using System.Text.Json.Serialization;
using Elsa.Common.Models;
using Elsa.Workflows.Serialization.Converters;

namespace Elsa.Workflows.Api.Endpoints.WorkflowDefinitions.BulkDispatch;

internal class Request
{
    public string DefinitionId { get; set; } = default!;
    public VersionOptions? VersionOptions { get; set; }
    public string? TriggerActivityId { get; set; }

    [JsonConverter(typeof(ExpandoObjectConverterFactory))]
    public object? Input { get; set; }

    public int Count { get; set; } = 1;
}

internal record Response(ICollection<string> WorkflowInstanceIds);