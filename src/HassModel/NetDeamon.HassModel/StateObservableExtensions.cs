using System.Reactive.Concurrency;

namespace NetDaemon.HassModel;

/// <summary>
/// Provides extension methods for <see cref="IObservable&lt;StateChange&gt;"/>
/// </summary>
public static class StateObservableExtensions
{
    /// <summary>
    /// Waits for an EntityState to match a predicate for the specified time
    /// </summary>
    // public static IObservable<StateChange> WhenStateIsFor(
    //     this IObservable<StateChange> observable, 
    //     Func<IEntityState<object>?, bool> predicate,
    //     TimeSpan timeSpan,
    //     IScheduler? scheduler = null)
    //
    //     => observable
    //         // Only process changes that start or stop matching the predicate
    //         .Where(e => predicate(e.Old) != predicate(e.New))
    //             
    //         // Both  will restart the timer
    //         .Throttle(timeSpan, scheduler ?? Scheduler.Default)
    //         
    //         // But only when the new state matches the predicate we emit it
    //         .Where(e => predicate(e.New));
    
    /// <summary>
    /// Waits for an EntityState to match a predicate for the specified time
    /// </summary>
    public static IObservable<TStateChange> WhenStateIsFor<TStateChange>(
        this IObservable<TStateChange> observable, 
        Func<IEntityState<object>?, bool> predicate,
        TimeSpan timeSpan,
        IScheduler? scheduler = null)
    where TStateChange : IStateChange<Entity, object>

        => observable
            .Where(e => predicate(e.Old) != predicate(e.New))
            .Throttle(timeSpan, scheduler ?? Scheduler.Default)
            .Where(e => predicate(e.New));
}
