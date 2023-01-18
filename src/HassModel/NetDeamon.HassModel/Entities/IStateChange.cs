namespace NetDaemon.HassModel.Entities;

public interface IStateChange<out TEntity, out TAttributes>
    where TEntity : IEntity<TEntity, TAttributes>
    where TAttributes : class
{
    TEntity Entity { get; }

    IEntityState<TAttributes>? New { get; }

    IEntityState<TAttributes>? Old { get; }
}