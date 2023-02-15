namespace NetDaemon.HassModel.Entities;

public interface IStateChange<out TEntity, out TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    TEntity Entity { get; }

    IEntityState<TAttributes>? New { get; }

    IEntityState<TAttributes>? Old { get; }
}

public interface INumericStateChange<out TEntity, out TAttributes> : IStateChange<TEntity, TAttributes>
    where TEntity : class, IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    INumericEntityState<TAttributes>? New { get; }

    INumericEntityState<TAttributes>? Old { get; }
    
}