namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Entity that has a numeric (double) State value
/// </summary>
public record NumericEntity : Entity, IEntity<NumericEntity, object>
{
    /// <summary>Copy constructor from base class</summary>
    public NumericEntity(IEntityCore entity) : base(entity) { }
    
    /// <summary>Constructor from haContext and entityId</summary>
    public NumericEntity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
        
    /// <summary>The current state of this Entity converted to double if possible, null if it is not</summary>
    public new double? State => EntityState?.State;

    /// <inheritdoc/>
    public override NumericEntityState? EntityState => base.EntityState == null ? null : new (base.EntityState);
        
    /// <inheritdoc/>
    public override IObservable<NumericStateChange<NumericEntity, object> > StateAllChanges() => 
        base.StateAllChanges().Select(e => new NumericStateChange<NumericEntity, object>(this, 
                MapState(e.Old), MapState(e.New)));
        
    /// <inheritdoc/>
    public override IObservable<NumericStateChange<NumericEntity, object>> StateChanges() => StateAllChanges().Where(e => e.Old?.State != e.New?.State);
    
    private static EntityState<Object>? MapState(IEntityState<object>? state) => state is null ? null : new EntityState<object>(state);

    
    IEntityState<object>? IEntity<NumericEntity, object>.EntityState => EntityState;

    IObservable<IStateChange<NumericEntity, object>> IEntity<NumericEntity, object>.StateAllChanges() => StateAllChanges();

    IObservable<IStateChange<NumericEntity, object>> IEntity<NumericEntity, object>.StateChanges() => StateChanges();
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
    // we need a new here because EntityState is not covariant for TAttributes

    /// <inheritdoc/>
    public override IObservable<NumericStateChange<TEntity, TAttributes>> StateAllChanges() => 
        base.StateAllChanges().Select(e => new NumericStateChange<TEntity, TAttributes>((TEntity)this, e.Old, e.New));

    /// <inheritdoc/>
    public override IObservable<NumericStateChange<TEntity, TAttributes>> StateChanges() =>
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