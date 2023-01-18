namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Represents a state change event for an entity
/// </summary>
public record StateChange
{
    /// <summary>
    /// This should not be used under normal circumstances but can be used for unit testing of apps
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="old"></param>
    /// <param name="new"></param>
    public StateChange(Entity entity, IEntityState<object>? old, IEntityState<object>? @new)
    {
        Entity = entity;
        New    = @new;
        Old    = old;
    }

    /// <summary>The Entity that changed</summary>
    public virtual Entity Entity { get; }

    /// <summary>The old state of the entity</summary>
    public virtual IEntityState<object>? Old { get; }

    /// <summary>The new state of the entity</summary>
    public virtual IEntityState<object>? New { get; }
}

/// <summary>
/// Represents a state change event for a strong typed entity and state 
/// </summary>
/// <typeparam name="TEntity">The Type</typeparam>
/// <typeparam name="TAttributes"></typeparam>
public record StateChange<TEntity, TAttributes> : StateChange, IStateChange<TEntity, TAttributes> 
    where TEntity : 
    //Entity, 
    IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// This should not be used under normal circumstances but can be used for unit testing of apps
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="old"></param>
    /// <param name="new"></param>
    public StateChange(TEntity entity, IEntityState<TAttributes>? old, IEntityState<TAttributes>? @new) : base(new Entity(entity), old, @new)
    {
        // todo: is a duplicate field really needed here?
        Entity = entity;
    }

    /// <inheritdoc/>
    public TEntity Entity { get; }

    /// <inheritdoc/>
    public override IEntityState<TAttributes>? New => (IEntityState<TAttributes>?)base.New;

    /// <inheritdoc/>
    public override IEntityState<TAttributes>? Old => (IEntityState<TAttributes>?)base.Old;
}