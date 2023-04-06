namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Entity that has a numeric (double) State value
/// </summary>
public record NumericEntity : Entity<NumericEntity, object>
{
    /// <summary>Copy constructor from base class</summary>
    public NumericEntity(IEntityCore entity) : base(entity) { }
    
    /// <summary>Constructor from haContext and entityId</summary>
    public NumericEntity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
        
    /// <summary>The current state of this Entity converted to double if possible, null if it is not</summary>
    public new double? State => EntityState?.State;

    /// <inheritdoc/>
    public new NumericEntityState<object>? EntityState => base.EntityState == null ? null : new (base.EntityState);
        
    /// <inheritdoc/>
    public new IObservable<INumericStateChange<NumericEntity, object> > StateAllChanges() => 
        base.StateAllChanges().Select(e => new NumericStateChange<NumericEntity, object>(this, 
                MapState(e.Old), MapState(e.New)));
        
    /// <inheritdoc/>
    public new IObservable<INumericStateChange<NumericEntity, object>> StateChanges() => 
        base.StateChanges().Select(e => new NumericStateChange<NumericEntity, object>(this, 
            MapState(e.Old), MapState(e.New)));
    
    private static EntityState<Object>? MapState(IEntityState<object>? state) => state is null ? null : new EntityState<object>(state);
}

// for backward compat in codegen
public record NumericEntity<TEntity, TEntiyState, TAttributes> : NumericEntity<TEntity, TAttributes>
    where TEntity : NumericEntity<TEntity, TAttributes>, IEntity<TEntity, TAttributes>
    where TEntiyState : NumericEntityState<TAttributes>
    where TAttributes : class
{
    public NumericEntity(IEntityCore entity) : base(entity)
    {
    }

    public NumericEntity(IHaContext haContext, string entityId) : base(haContext, entityId)
    {
    }
}

/// <summary>
/// Entity that has a numeric (double) State value
/// </summary>
public record NumericEntity<TEntity, TAttributes> : 
    Entity<TEntity, EntityState<TAttributes>, TAttributes>
    where TEntity : NumericEntity<TEntity, TAttributes>, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    /// <summary>Copy constructor from base class</summary>
    public NumericEntity(IEntityCore entity) : base(entity) { }

    /// <summary>Constructor from haContext and entityId</summary>
    public NumericEntity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
        
    /// <summary>The current state of this Entity converted to double if possible, null if it is not</summary>
    public new double? State => EntityState?.State;
        
    /// <summary>The full state of this Entity</summary>
    public new NumericEntityState<TAttributes>? EntityState => base.EntityState == null ? null : new (base.EntityState);

    /// <inheritdoc/>
    public new IObservable<NumericStateChange<TEntity, TAttributes>> StateAllChanges() => 
        base.StateAllChanges().Select(e => new NumericStateChange<TEntity, TAttributes>((TEntity)this, e.Old, e.New));

    /// <inheritdoc/>
    public new IObservable<NumericStateChange<TEntity, TAttributes>> StateChanges() =>
        StateAllChanges().Where(e => e.New?.State != e.Old?.State);
}
    
/// <summary>
/// Entity that has a numeric (double) State value
/// </summary>
public record NumericEntity<TAttributes> : NumericEntity<NumericEntity<TAttributes>, TAttributes>
    where TAttributes : class
{
    // This type is needed because the base type has a recursive type parameter so it can not be used as a return value
        
    /// <summary>Copy constructor from base class</summary>
    public NumericEntity(IEntityCore entity) : base(entity) { }
    
    /// <summary>Constructor from haContext and entityId</summary>
    public NumericEntity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
}