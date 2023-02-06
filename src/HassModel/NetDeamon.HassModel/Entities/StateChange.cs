namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Represents a state change event for an entity
/// </summary>
public record StateChange : StateChange<Entity, object> 
{
    /// <summary>
    /// This should not be used under normal circumstances but can be used for unit testing of apps
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="old"></param>
    /// <param name="new"></param>
    public StateChange(Entity entity, IEntityState<object>? old, IEntityState<object>? @new) : base(entity, old, @new)
    {

    }
}

/// <summary>
/// Represents a state change event for a strong typed entity and state 
/// </summary>
/// <typeparam name="TEntity">The Type</typeparam>
/// <typeparam name="TAttributes"></typeparam>
public record StateChange<TEntity, TAttributes> : IStateChange<TEntity, TAttributes> 
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    public StateChange(TEntity entity, IEntityState<object>? old, IEntityState<object>? @new) :
        this(entity, EntityState<TAttributes>.MapState(old), EntityState<TAttributes>.MapState(@new))
    {
    }


    /// <summary>
    /// This should not be used under normal circumstances but can be used for unit testing of apps
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="old"></param>
    /// <param name="new"></param>
    public StateChange(TEntity entity, IEntityState<TAttributes>? old, IEntityState<TAttributes>? @new)
    {
        Entity = entity;
        New    = @new;
        Old    = old;
    }

    /// <inheritdoc/>
    public virtual TEntity Entity { get; }

    /// <inheritdoc/>
    public virtual IEntityState<TAttributes>? New { get; }


    /// <inheritdoc/>
    public virtual IEntityState<TAttributes>? Old { get; }

}