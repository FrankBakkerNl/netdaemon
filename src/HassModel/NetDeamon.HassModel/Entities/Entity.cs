namespace NetDaemon.HassModel.Entities;

/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public record Entity : IEntity<Entity, object>
{
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

    /// <summary>The current state of this Entity</summary>
    public string? State => EntityState?.State;

    
    // Implement these explicit to avoid cluttering 
    JsonElement? IEntityState.AttributesJson => EntityState?.AttributesJson;

    DateTime? IEntityState.LastChanged => EntityState?.LastChanged;

    DateTime? IEntityState.LastUpdated => EntityState?.LastUpdated;

    Context? IEntityState.Context => EntityState?.Context;

    /// <summary>
    /// The current Attributes of this Entity
    /// </summary>
    public virtual object? Attributes => EntityState?.Attributes;

    /// <summary>
    /// The full state of this Entity
    /// </summary>
    public virtual IEntityState<object>? EntityState => HaContext.GetState(EntityId);

    /// <summary>
    /// Observable, All state changes including attributes
    /// </summary>
    public virtual IObservable<IStateChange<Entity, object>> StateAllChanges() =>
        HaContext.StateAllChanges().Where(e => e.Entity.EntityId == EntityId).Select(e => new StateChange<Entity, object>(this, e.Old,e.New));

    /// <summary>
    /// Observable, All state changes. New.State!=Old.State
    /// </summary>
    public virtual IObservable<IStateChange<Entity, object>> StateChanges() =>
        StateAllChanges().StateChangesOnlyFilter();

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

/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public abstract record Entity<TEntity, TAttributes> : Entity,
    IEntity<TEntity, TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    protected Entity(IEntityCore entity) : this(entity.HaContext, entity.EntityId)
    { }

    protected Entity(IHaContext haContext, string entityId) : base(haContext, entityId)
    { }

    public override IEntityState<TAttributes>? EntityState => new EntityState<TAttributes>(base.EntityState);
    public override TAttributes? Attributes => EntityState?.Attributes;

    public new IObservable<IStateChange<TEntity, TAttributes>> StateAllChanges()
        => base.StateAllChanges().Select(e => new StateChange<TEntity, TAttributes>((TEntity)(object)this, e.Old,e.New));

    public new IObservable<IStateChange<TEntity, TAttributes>> StateChanges() => StateAllChanges().StateChangesOnlyFilter();
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
    
    /// <summary>Constructor from haContext and entityId</summary>
    public Entity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
}