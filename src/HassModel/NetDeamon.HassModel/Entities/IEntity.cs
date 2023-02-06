namespace NetDaemon.HassModel.Entities;

public interface IEntityCore
{
    /// <summary>
    /// The IHAContext
    /// </summary>
    IHaContext HaContext { get; }

    /// <summary>
    /// Entity id being handled by this entity
    /// </summary>
    string EntityId { get; }
}

public interface IEntity<out TEntity, out TAttributes>
    : IEntityCore//, IEntityState<TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// Area name of entity
    /// </summary>
    string? Area { get; }

    /// <inheritdoc />
    IEntityState<TAttributes>? EntityState { get; }

    /// <summary>The current state of this Entity</summary>
    string? State { get; }

    /// <summary>
    /// The current Attributes of this Entity
    /// </summary>
    object? Attributes { get; }

    /// <inheritdoc />
    IObservable<IStateChange<TEntity, TAttributes>> StateAllChanges();

    /// <inheritdoc />
    IObservable<IStateChange<TEntity, TAttributes>> StateChanges();

    /// <summary>
    /// Calls a service using this entity as the target
    /// </summary>
    /// <param name="service">Name of the service to call. If the Domain of the service is the same as the domain of the Entity it can be omitted</param>
    /// <param name="data">Data to provide</param>
    void CallService(string service, object? data = null);
}
