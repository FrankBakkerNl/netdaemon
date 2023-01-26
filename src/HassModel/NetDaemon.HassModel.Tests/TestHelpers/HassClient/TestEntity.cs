using System.Text.Json.Serialization;
using NetDaemon.HassModel.Entities;

namespace NetDaemon.HassModel.Tests.TestHelpers.HassClient;

public record TestEntity : Entity<TestEntity, TestEntityAttributes>, IEntity<TestEntity, TestEntityAttributes>
{
    public TestEntity(IHaContext haContext, string entityId) : base(haContext, entityId) { }
}


public record TestEntity2: 
     Entity<TestEntity2, object>,
     IEntity<TestEntity2, TestEntityAttributes>
{
    
    public IHaContext HaContext { get; }
    public string EntityId { get; }
    public string? Area { get; }
    public IEntityState<TestEntityAttributes>? EntityState { get; }
    public IObservable<IStateChange<TestEntity, TestEntityAttributes>> StateAllChanges()
    {
        throw new NotImplementedException();
    }

    public IObservable<IStateChange<TestEntity, TestEntityAttributes>> StateChanges()
    {
        throw new NotImplementedException();
    }

    public void CallService(string service, object? data = null)
    {
        throw new NotImplementedException();
    }
}


public record TestEntityAttributes
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
}

public record NumericTestEntity : NumericEntity<NumericTestEntity, TestEntityAttributes>
{
    public NumericTestEntity(Entity entity) : base(entity)
    { }

    public NumericTestEntity(IHaContext haContext, string entityId) : base(haContext, entityId)
    { }
}