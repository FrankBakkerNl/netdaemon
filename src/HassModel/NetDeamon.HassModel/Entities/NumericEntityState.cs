namespace NetDaemon.HassModel.Entities;

// /// <summary>
// /// State for a Numeric Entity
// /// </summary>
// public record NumericEntityState : EntityState, INumericEntityState<object>
// {
//     /// <summary>Copy constructor from base class</summary>
//     public NumericEntityState(IEntityState<object> source) : base(source) { }
//
//     /// <summary>The state converted to double if possible, null if it is not</summary>
//     public new double? State => FormatHelpers.ParseAsDouble(base.State);
// }

/// <summary>
/// State for a Numeric Entity with specific types of Attributes
/// </summary>
public record NumericEntityState<TAttributes> : EntityState<TAttributes>, INumericEntityState<TAttributes>
    where TAttributes : class
{
    /// <summary>Copy constructor from base class</summary>
    public NumericEntityState(IEntityState<TAttributes> source) : base(source)
    { }

    /// <summary>The state converted to double if possible, null if it is not</summary>
    public new double? State => FormatHelpers.ParseAsDouble(base.State);
}
    
// /// <summary>
// /// Represents a state change event for a strong typed entity and state 
// /// </summary>
// public record NumericStateChange : StateChange<NumericEntity, object>
// {
//     internal NumericStateChange(NumericEntity entity, NumericEntityState? old, NumericEntityState? @new) : base(entity, old, @new)
//     { }
//     
//     public new NumericEntityState? New => base.New is null ? null : new(base.New);
//     public new NumericEntityState? Old => base.Old is null ? null : new(base.Old);
// }
    
/// <summary>
/// Represents a state change event for a strong typed entity and state 
/// </summary>
public record NumericStateChange<TEntity, TAttributes> : StateChange<TEntity, TAttributes> , INumericStateChange<TEntity, TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    internal NumericStateChange(TEntity entity, IEntityState<TAttributes>? old, IEntityState<TAttributes>? @new) : base(entity, old, @new)
    { }

    /// <summary>The state converted to double if possible, null if it is not</summary>
    public new INumericEntityState<TAttributes>? New => base.New is null ? null : new NumericEntityState<TAttributes>(base.New);
    public new INumericEntityState<TAttributes>? Old => base.Old is null ? null : new NumericEntityState<TAttributes>(base.Old);
}