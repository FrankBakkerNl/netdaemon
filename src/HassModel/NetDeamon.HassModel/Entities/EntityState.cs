namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Detailed state information
/// </summary>
public record EntityState : IEntityState<object>
{
    public EntityState()
    { }
    
    public EntityState(IEntityState<object> source)
    {
        EntityId = source.EntityId;
        State = source.State;
        AttributesJson = source.AttributesJson;
        LastChanged = source.LastChanged;
        LastUpdated = source.LastUpdated;
        Context = source.Context;
    }
    
    /// <summary>Unique id of the entity</summary>
    public string EntityId { get; init; } = "";
    
    /// <summary>The state </summary>
    public string? State { get; init; }

    /// <summary>The attributes as a JsonElement</summary>
    public JsonElement? AttributesJson { get; init; }
        
    /// <summary>
    /// The attributes
    /// </summary>
    public virtual object? Attributes => AttributesJson?.Deserialize<Dictionary<string, object>>() ?? new Dictionary<string, object>();
    
    /// <summary>Last changed, when state changed from and to different values</summary>
    public DateTime? LastChanged { get; init; }
    
    /// <summary>Last updated, when entity state or attributes changed </summary>
    public DateTime? LastUpdated { get; init; }
    
    /// <summary>Context</summary>
    public Context? Context { get; init; }
        
}

/// <summary>
/// Generic EntityState with specific types of State and Attributes
/// </summary>
/// <typeparam name="TAttributes">The type of the Attributes Property</typeparam>
public record EntityState<TAttributes> : EntityState, IEntityState<TAttributes>
    where TAttributes : class
{
    private readonly Lazy<TAttributes?> _attributesLazy;

    internal static EntityState<TAttributes>? MapState(IEntityState<object>? state) => state is null ? null : new EntityState<TAttributes>(state);
    
    /// <summary>
    /// Copy constructor from base class
    /// </summary>
    /// <param name="source"></param>
    public EntityState(IEntityState<object> source) : base(source)
    {
        _attributesLazy = new (() => AttributesJson?.Deserialize<TAttributes>() ?? default);            
    }

    /// <inheritdoc/>
    public override TAttributes? Attributes => _attributesLazy.Value;
}