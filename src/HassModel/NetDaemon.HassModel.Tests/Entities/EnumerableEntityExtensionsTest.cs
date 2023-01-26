using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NetDaemon.HassModel.Entities;
using NetDaemon.HassModel.Tests.TestHelpers;
using NetDaemon.HassModel.Tests.TestHelpers.HassClient;

namespace NetDaemon.HassModel.Tests.Entities;

public class EnumerableEntityExtensionsTest
{
    [Fact]
    public void TestStateChanges()
    {
        Subject<StateChange> stateChangesSubject = new();
        var haMock = new Mock<IHaContext>();
        haMock.Setup(h => h.StateAllChanges()).Returns(stateChangesSubject);

        var switch1 = new Entity(haMock.Object, "switch.Living1");
        var switch2 = new Entity(haMock.Object, "switch.Living2");

        // Act: Subscribe to both entities
        var observerMock = new[] { switch1, switch2 }.StateChanges().SubscribeMock();

        stateChangesSubject.OnNext(new StateChange(switch1, new EntityState { State = "OldState1" }, new EntityState { State = "NewState1" }));

        observerMock.Verify(m => m.OnNext(It.Is<IStateChange<Entity, object>>(s => s.Entity == switch1 && s.New!.State == "NewState1")), Times.Once);
        observerMock.VerifyNoOtherCalls();

        stateChangesSubject.OnNext(new StateChange(switch2, new EntityState { State = "OldState2" }, new EntityState { State = "NewState2" }));

        observerMock.Verify(m => m.OnNext(It.Is<IStateChange<Entity, object>>(s => s.Entity == switch2 && s.New!.State == "NewState2")), Times.Once);
        observerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void TestTypedStateChanges()
    {

        Subject<StateChange> stateChangesSubject = new();
        var haMock = new Mock<IHaContext>();
        haMock.Setup(h => h.StateAllChanges()).Returns(stateChangesSubject);

        var switch1 = new TestEntity2(haMock.Object, "switch.Living1");
        var switch2 = new TestEntity2(haMock.Object, "switch.Living2");

        // Act: Subscribe to both entities, filter on attribute
        // TODO: remove type arguments here
        var testEntities = new[] { switch1, switch2 };
        var observerMock = EnumerableEntityExtensions.StateAllChanges(testEntities).Where(e => e.New?.Attributes?.Name == "Do").SubscribeMock();
        
        stateChangesSubject.OnNext(new StateChange(switch1,
            new EntityState { State = "State", AttributesJson = new { name = "John" }.AsJsonElement() },
            new EntityState { State = "State", AttributesJson = new { name = "Do" }.AsJsonElement() }
        ));

        observerMock.Verify(m => m.OnNext(It.Is<StateChange<TestEntity, TestEntityAttributes>>(s => s.Entity == switch1 && s.New!.State == "State")), Times.Once);
        observerMock.VerifyNoOtherCalls();

        stateChangesSubject.OnNext(new StateChange(switch2, new EntityState { State = "OldState2" }, new EntityState { State = "NewState2" }));

        observerMock.Verify(m => m.OnNext(It.IsAny<StateChange<TestEntity, TestEntityAttributes>>()), Times.Once);
    }

    [Fact]
    public void TestCallService()
    {
        var haMock = new Mock<IHaContext>();

        var switch1 = new Entity(haMock.Object, "switch.Living1");
        var switch2 = new Entity(haMock.Object, "switch.Living2");

        // Act:
        var data = new { Name = "John", Age = 12 };
        new[] { switch1, switch2 }.CallService("set_state", data);

        haMock.Verify(m => m.CallService("switch", "set_state", It.IsAny<ServiceTarget>(), data));
        haMock.Invocations.First().Arguments[2].As<ServiceTarget>().EntityIds
            .Should().BeEquivalentTo("switch.Living1", "switch.Living2");
        haMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void TestCallServiceWithDomainInService()
    {
        var haMock = new Mock<IHaContext>();

        var switch1 = new Entity(haMock.Object, "switch.Living1");
        var switch2 = new Entity(haMock.Object, "light.Living2");

        // Act:
        var data = new { Name = "John", Age = 12 };
        new[] { switch1, switch2 }.CallService("homeassistant.turn_on", data);

        haMock.Verify(m => m.CallService("homeassistant", "turn_on", It.IsAny<ServiceTarget>(), data));
        haMock.Invocations.First().Arguments[2].As<ServiceTarget>().EntityIds
            .Should().BeEquivalentTo("switch.Living1", "light.Living2");
        haMock.VerifyNoOtherCalls();
    }    
    
    [Fact]
    public void TestCallServiceWithDifferentDomainsNotAllowed()
    {
        var haMock = new Mock<IHaContext>();

        var switch1 = new Entity(haMock.Object, "switch.Living1");
        var switch2 = new Entity(haMock.Object, "light.Living2");

        // Act:
        var data = new { Name = "John", Age = 12 };
        var action = () => new[] { switch1, switch2 }.CallService("turn_on", data);
        action.Should().Throw<InvalidOperationException>();
    }    
}