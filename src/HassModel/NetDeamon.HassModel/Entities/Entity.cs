namespace NetDaemon.HassModel.Entities;

/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public record Entity : IEntityCore, IEntity<Entity, object>
{
    private IEntityState<object>? _entityState;

    /// <summary>
    /// The IHAContext
    /// </summary>
    public IHaContext HaContext { get; }

    /// <summary>
    /// Entity id being handled by this entity
    /// </summary>
    public string EntityId { get; }

    public Entity(IEntityCore core) : this(core.HaContext, core.EntityId) { }

    /// <summary>
    /// Creates a new instance of a Entity class
    /// </summary>
    /// <param name="haContext">The Home Assistant context associated with this Entity</param>
    /// <param name="entityId">The id of this Entity</param>
    public Entity(IHaContext haContext, string entityId)
    {
        HaContext = haContext;
        EntityId = entityId;
    }
        
    /// <summary>
    /// Area name of entity
    /// </summary>
    public string? Area => HaContext.GetAreaFromEntityId(EntityId)?.Name;

    IEntityState<object>? IEntity<Entity, object>.EntityState => EntityState;

    IObservable<IStateChange<Entity, object>> IEntity<Entity, object>.StateAllChanges()
        => StateAllChanges().Select(c => new StateChange<Entity, object>(
            this, 
            EntityState<Object>.MapState(c.Old),
            EntityState<Object>.MapState(c.New)));

    IObservable<IStateChange<Entity, object>> IEntity<Entity, object>.StateChanges() => StateAllChanges().StateChangesOnlyFilter();

    /// <summary>The current state of this Entity</summary>
    public string? State => EntityState?.State;

    /// <summary>
    /// The current Attributes of this Entity
    /// </summary>
    public virtual object? Attributes => EntityState?.Attributes;

    /// <summary>
    /// The full state of this Entity
    /// </summary>
    public EntityState? EntityState => HaContext.GetState(EntityId);

    /// <summary>
    /// Observable, All state changes including attributes
    /// </summary>
    public IObservable<StateChange> StateAllChanges() =>
        HaContext.StateAllChanges().Where(e => e.Entity.EntityId == EntityId);

    /// <summary>
    /// Observable, All state changes. New.State!=Old.State
    /// </summary>
    public virtual IObservable<StateChange> StateChanges() =>
        StateAllChanges().Where(e => e.New?.State != e.Old?.State);

    /// <summary>
    /// Calls a service using this entity as the target
    /// </summary>
    /// <param name="service">Name of the service to call. If the Domain of the service is the same as the domain of the Entity it can be omitted</param>
    /// <param name="data">Data to provide</param>
    public virtual void CallService(string service, object? data = null)
    {
        ArgumentNullException.ThrowIfNull(service, nameof(service));
            
        var (serviceDomain, serviceName) = service.SplitAtDot();

        serviceDomain ??= EntityId.SplitAtDot().Left ?? throw new InvalidOperationException("EntityId must be formatted 'domain.name'");
            
        HaContext.CallService(serviceDomain, serviceName, ServiceTarget.FromEntity(EntityId), data);
    }
}

public abstract record Entity<TEntity, TAttributes> :
    IEntity<TEntity, TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    protected Entity(IEntityCore entity) : base(entity)
    {
    }

    protected Entity(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }

    protected Entity() : base(BASE)
    {
        throw new NotImplementedException();
    }

    public IHaContext HaContext { get; }
    public string EntityId { get; }
    public string? Area { get; }
    public IEntityState<TAttributes>? EntityState { get; }
    public IObservable<IStateChange<TEntity, TAttributes>> StateAllChanges()
    {
        throw new NotImplementedException();
    }

    public IObservable<IStateChange<TEntity, TAttributes>> StateChanges()
    {
        throw new NotImplementedException();
    }

    public void CallService(string service, object? data = null)
    {
        throw new NotImplementedException();
    }
}




/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public abstract record Entity<TEntity, TEntityState, TAttributes> : Entity, IEntity<TEntity, TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TEntityState : EntityState<TAttributes>
    where TAttributes : class
{
    /// <summary>Copy constructor from Base type</summary>
    protected Entity(IEntityCore entity) : base(entity)
    { }

    /// <summary>Constructor from haContext and entityId</summary>
    protected Entity(IHaContext haContext, string entityId) : base(haContext, entityId)
    { }

    /// <inheritdoc />
    public override TAttributes? Attributes => EntityState?.Attributes;

    /// <inheritdoc />
    public new IEntityState<TAttributes>? EntityState => EntityState<TAttributes>.MapState(base.EntityState);

    /// <inheritdoc />
    public new IObservable<IStateChange<TEntity, TAttributes>> StateAllChanges() =>
        base.StateAllChanges().Select(e => new StateChange<TEntity, TAttributes>((TEntity)(object)this,
            EntityState<TAttributes>.MapState(e.Old), EntityState<TAttributes>.MapState(e.New)));

    /// <inheritdoc />
    public new IObservable<IStateChange<TEntity, TAttributes>> StateChanges() => StateAllChanges().StateChangesOnlyFilter();

}    


/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public record Entity<TAttributes> : Entity<Entity<TAttributes>, EntityState<TAttributes>, TAttributes>
    where TAttributes : class
{
    // This type is needed because the base type has a recursive type parameter so it can not be used as a return value
        
    /// <summary>Copy constructor from Base type</summary>
    public Entity(IEntityCore entity) : base(entity) { }
}