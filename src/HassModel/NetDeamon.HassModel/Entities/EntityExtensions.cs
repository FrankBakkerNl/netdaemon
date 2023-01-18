namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Provides Extension methods for Entities
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    /// Checks if en EntityState has the state "on" 
    /// </summary>
    /// <param name="entityState">The state to check</param>
    /// <returns>true if the state equals "on", otherwise false</returns>
    public static bool IsOn(this IEntityState<object>? entityState) => string.Equals(entityState?.State, "on", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if en EntityState has the state "off" 
    /// </summary>
    /// <param name="entityState">The state to check</param>
    /// <returns>true if the state equals "off", otherwise false</returns>
    public static bool IsOff(this IEntityState<object>? entityState) => string.Equals(entityState?.State, "off", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if en Entity has the state "on" 
    /// </summary>
    /// <param name="entity">The state to check</param>
    /// <returns>true if the state equals "on", otherwise false</returns>
    public static bool IsOn(this Entity? entity) => entity?.EntityState?.IsOn() ?? false;

    /// <summary>
    /// Checks if en Entity has the state "off" 
    /// </summary>
    /// <param name="entity">The state to check</param>
    /// <returns>true if the state equals "off", otherwise false</returns>
    public static bool IsOff(this Entity? entity) => entity?.EntityState?.IsOff() ?? false;

    /// <summary>Gets a NumericEntity from a given Entity</summary>
    public static NumericEntity AsNumeric(this Entity entity) => new(entity);
    
    /// <summary>Gets a NumericEntity from a given Entity</summary>
    public static NumericEntity<TAttributes> 
        AsNumeric<TAttributes>(this Entity<TAttributes> entity)
        where TAttributes : class
        => new(entity);

    /// <summary>Gets a new Entity from this Entity with the specified type of attributes</summary>
    public static Entity<TNewAttributes> WithAttributesAs<TNewAttributes>(this IEntityCore entity)
        where TNewAttributes : class
        => new Entity<TNewAttributes>(entity);

    /// <summary>Gets a new Entity from this Entity with the specified type of attributes</summary>
    public static NumericEntity<TAttributes> WithAttributesAs<TAttributes>(this NumericEntity entity)
        where TAttributes : class
        => new (entity);

    internal static IObservable<IStateChange<TEntity, TAttributes>> StateChangesOnly<TEntity, TAttributes>
        (this IObservable<IStateChange<TEntity, TAttributes>> changes) 
        where TEntity : IEntity<TEntity, TAttributes>
        where TAttributes : class
        => changes.Where(c => c.New?.State != c.Old?.State);
    
    
    internal static IObservable<StateChange<TEntity, TAttributes>> StateChangesOnly<TEntity, TAttributes>
        (this IObservable<StateChange<TEntity, TAttributes>> changes) 
        where TEntity : IEntity<TEntity, TAttributes>
        where TAttributes : class
        => changes.Where(c => c.New?.State != c.Old?.State);
    
}