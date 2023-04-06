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
    public static bool IsOn(this IEntityState? entityState) => string.Equals(entityState?.State, "on", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if en EntityState has the state "off" 
    /// </summary>
    /// <param name="entityState">The state to check</param>
    /// <returns>true if the state equals "off", otherwise false</returns>
    public static bool IsOff(this IEntityState? entityState) => string.Equals(entityState?.State, "off", StringComparison.OrdinalIgnoreCase);

    public static double? StateAsDouble(this IEntityState? entityState) => FormatHelpers.ParseAsDouble(entityState?.State); 

    /// <summary>Gets a NumericEntity from a given Entity</summary>
    public static NumericEntity AsNumeric(this Entity entity) => new(entity);
    
    /// <summary>Gets a NumericEntity from a given Entity</summary>
    public static NumericEntity<TAttributes> 
        AsNumeric<TAttributes>(this Entity<TAttributes> entity)
        where TAttributes : class
        => new(entity);

    /// <summary>Gets a new Entity from this Entity with the specified type of attributes</summary>
    public static Entity<TAttributes> WithAttributesAs<TAttributes>(this IEntityCore entity)
        where TAttributes : class
        => new (entity);

    /// <summary>Gets a new Entity from this Entity with the specified type of attributes</summary>
    public static NumericEntity<TAttributes> WithAttributesAs<TAttributes>(this NumericEntity entity)
        where TAttributes : class
        => new (entity);

    internal static IObservable<IStateChange<TEntity, TAttributes>> StateChangesOnlyFilter<TEntity, TAttributes>(
        this IObservable<IStateChange<TEntity, TAttributes>> changes)
        where TEntity : class, IEntity<TEntity, TAttributes>
        where TAttributes : class
        => changes.Where(c => c.New?.State != c.Old?.State);

}