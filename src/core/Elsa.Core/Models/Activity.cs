using System.Linq.Expressions;
using Elsa.Contracts;
using Elsa.Helpers;
using Elsa.Signals;

namespace Elsa.Models;

public abstract class Activity : ISignalHandler
{
    private readonly ICollection<SignalHandlerRegistration> _signalHandlers = new List<SignalHandlerRegistration>();
    protected Activity()
    {
        TypeName = TypeNameHelper.GenerateTypeName(GetType());
        OnSignalReceived<ActivityCompleted>(OnChildActivityCompletedAsync);
    }

    protected Activity(string activityType) : this()
    {
        TypeName = activityType;
    }

    public string Id { get; set; } = default!;
    public string TypeName { get; set; }
    public bool CanStartWorkflow { get; set; }
    public IDictionary<string, object> ApplicationProperties { get; set; } = new Dictionary<string, object>();
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// A value indicating whether this activity should complete automatically.
    /// Default is true.
    /// </summary>
    protected virtual bool CompleteImplicitly => true;

    protected virtual async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Execute(context);
        
        if(CompleteImplicitly)
            await CompleteAsync(context);
    }

    protected virtual ValueTask OnSignalReceivedAsync(object signal, SignalContext context)
    {
        OnSignalReceived(signal, context);
        return ValueTask.CompletedTask;
    }

    protected virtual void OnSignalReceived(object signal, SignalContext context)
    {
    }

    protected virtual void Execute(ActivityExecutionContext context)
    {
    }

    /// <summary>
    /// Notify the sytem that this activity completed.
    /// </summary>
    protected async ValueTask CompleteAsync(ActivityExecutionContext context)
    {
        await context.CompleteActivityAsync();
    }

    protected void OnSignalReceived(Type signalType, Func<object, SignalContext, ValueTask> handler) => _signalHandlers.Add(new SignalHandlerRegistration(signalType, handler));
    protected void OnSignalReceived<T>(Func<T, SignalContext, ValueTask> handler) => OnSignalReceived(typeof(T), (signal, context) => handler((T)signal, context));

    protected void OnSignalReceived<T>(Action<T, SignalContext> handler)
    {
        OnSignalReceived<T>((signal, context) =>
        {
            handler(signal, context);
            return ValueTask.CompletedTask;
        });
    }
    
    protected virtual async ValueTask OnChildActivityCompletedAsync(ActivityCompleted signal, SignalContext context)
    {
        var activityExecutionContext = context.ActivityExecutionContext;
        var childActivityExecutionContext = context.SourceActivityExecutionContext;
        var childActivity = childActivityExecutionContext.Activity;
        var callbackEntry = activityExecutionContext.WorkflowExecutionContext.PopCompletionCallback(activityExecutionContext, childActivity);

        if (callbackEntry == null)
            return;

        await callbackEntry(activityExecutionContext, childActivityExecutionContext);
    }

    async ValueTask IActivity.ExecuteAsync(ActivityExecutionContext context)
    {
        await ExecuteAsync(context);
    }

    async ValueTask ISignalHandler.HandleSignalAsync(object signal, SignalContext context)
    {
        // Give derived activity a chance to do something with the signal.
        await OnSignalReceivedAsync(signal, context);

        // Invoke registered signal delegates for this particular type of signal.
        var signalType = signal.GetType();
        var handlers = _signalHandlers.Where(x => x.SignalType == signalType);

        foreach (var registration in handlers)
            await registration.Handler(signal, context);
    }
}

public abstract class ActivityWithResult : Activity
{
    protected ActivityWithResult()
    {
    }

    protected ActivityWithResult(string activityType) : base(activityType)
    {
    }

    protected ActivityWithResult(RegisterLocationReference? outputTarget)
    {
        if (outputTarget != null) this.CaptureOutput(outputTarget);
    }

    public Output Result { get; } = new();
}

public abstract class Activity<T> : ActivityWithResult
{
    protected Activity()
    {
    }

    protected Activity(string activityType) : base(activityType)
    {
    }

    protected Activity(RegisterLocationReference? outputTarget) : base(outputTarget)
    {
    }
}

public static class ActivityWithResultExtensions
{
    public static T CaptureOutput<T>(this T activity, Expression<Func<T, Output>> propertyExpression, RegisterLocationReference locationReference) where T : IActivity
    {
        var output = activity.GetPropertyValue(propertyExpression)!;
        output.Targets.Add(locationReference);
        return activity;
    }

    public static T CaptureOutput<T>(this T activity, RegisterLocationReference locationReference) where T : ActivityWithResult => activity.CaptureOutput(x => x.Result, locationReference);
}

internal record SignalHandlerRegistration(Type SignalType, Func<object, SignalContext, ValueTask> Handler);