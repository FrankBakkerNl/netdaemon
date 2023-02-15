namespace NetDaemon.HassModel.Entities;

public interface IEntityState<out TAttributes> : IEntityStateCore
    where TAttributes : class
{
    /// <inheritdoc/>
    TAttributes? Attributes { get; }
    
    /// <summary>Unique id of the entity</summary>
    string EntityId { get; }

    /// <summary>The state </summary>
    string? State { get; }

    /// <summary>The attributes as a JsonElement</summary>
    JsonElement? AttributesJson { get;}

    /// <summary>Last changed, when state changed from and to different values</summary>
    DateTime? LastChanged { get; }

    /// <summary>Last updated, when entity state or attributes changed </summary>
    DateTime? LastUpdated { get; }

    /// <summary>Context</summary>
    Context? Context { get; }
}

public interface INumericEntityState<out TAttributes> : IEntityState<TAttributes>
    where TAttributes : class
{

    /// <summary>The state </summary>
    double? State { get; }
}