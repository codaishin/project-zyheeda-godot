using System;
using System.Linq;
using Godot;
using NUnit.Framework;

[TestFixture]
public class TestBehaviorNode : TestCollection {
	private class MockBehaviorStateMachine : IBehaviorStateMachine {
		public Action onReset = () => { };
		public Action onRunNext = () => { };

		public void Reset() {
			this.onReset();
		}

		public void Run() {
			this.onRunNext();
		}
	}

	private class MockEquipment : Node, IEquipment {
		public static Func<Node, IBehaviorStateMachine> onGetBehaviorsFor =
			_ => new MockBehaviorStateMachine();

		public IBehaviorStateMachine GetBehaviorsFor(Node agent) {
			return MockEquipment.onGetBehaviorsFor(agent);
		}
	}

	[SetUp]
	public void ResetMockEquipment() {
		MockEquipment.onGetBehaviorsFor = _ => new MockBehaviorStateMachine();
	}

	[Test]
	public void GetBehaviorForAgentOnInit() {
		var agentArg = null as Node;
		MockEquipment.onGetBehaviorsFor = a => {
			agentArg = a;
			return new MockBehaviorStateMachine();
		};

		var agent = new Node { Name = "Agent" };
		Tests.BaseNode.AddChild(agent);

		var equipment = new MockEquipment { Name = "Equipment" };
		var scene = new PackedScene();
		_ = scene.Pack(equipment);
		equipment.QueueFree();

		var behavior = new BehaviorNode {
			Name = "Behavior",
			agent = agent.GetPath(),
			equipment = scene,
		};
		Tests.BaseNode.AddChild(behavior);

		behavior.Init();

		Assert.AreSame(agent, agentArg);
	}

	[Test]
	public void EquipmentInstanceBecomesBehaviorChild() {
		var agent = new Node { Name = "Agent" };
		Tests.BaseNode.AddChild(agent);

		var equipment = new MockEquipment { Name = "Equipment" };
		var packedEquipment = new PackedScene();
		_ = packedEquipment.Pack(equipment);
		equipment.QueueFree();

		var behavior = new BehaviorNode {
			Name = "Behavior",
			agent = agent.GetPath(),
			equipment = packedEquipment,
		};
		Tests.BaseNode.AddChild(behavior);

		behavior.Init();

		CollectionAssert.Contains(
			behavior.GetChildren().OfType<Node>().Select(n => n.Name),
			"Equipment"
		);
	}

	[Test]
	public void RunNextOnRun() {
		var called = 0;
		var states = new MockBehaviorStateMachine {
			onRunNext = () => ++called,
		};
		MockEquipment.onGetBehaviorsFor = _ => states;

		var agent = new Node { Name = "Agent" };
		Tests.BaseNode.AddChild(agent);

		var equipment = new MockEquipment { Name = "Equipment" };
		var scene = new PackedScene();
		_ = scene.Pack(equipment);
		equipment.QueueFree();

		var behavior = new BehaviorNode {
			Name = "Behavior",
			agent = agent.GetPath(),
			equipment = scene,
		};
		Tests.BaseNode.AddChild(behavior);

		behavior.Init();
		behavior.Run();

		Assert.AreEqual(1, called);
	}

	[Test]
	public void RunThrowsWhenInitNotCalled() {
		var behavior = new BehaviorNode { Name = "Behavior" };
		Tests.BaseNode.AddChild(behavior);

		var exception = Assert.Throws<FaultyExecutionOrder>(behavior.Run);
		Assert.AreEqual("Did you forget to call .Init()?", exception!.Message);
	}

	[Test]
	public void ResetOnCancel() {
		var called = 0;
		var states = new MockBehaviorStateMachine {
			onReset = () => ++called,
		};
		MockEquipment.onGetBehaviorsFor = _ => states;

		var agent = new Node { Name = "Agent" };
		Tests.BaseNode.AddChild(agent);

		var equipment = new MockEquipment { Name = "Equipment" };
		var scene = new PackedScene();
		_ = scene.Pack(equipment);
		equipment.QueueFree();

		var behavior = new BehaviorNode {
			Name = "Behavior",
			agent = agent.GetPath(),
			equipment = scene,
		};
		Tests.BaseNode.AddChild(behavior);

		behavior.Init();
		behavior.Cancel();

		Assert.AreEqual(1, called);
	}

	[Test]
	public void CancelThrowsWhenInitNotCalled() {
		var behavior = new BehaviorNode { Name = "Behavior" };
		Tests.BaseNode.AddChild(behavior);

		var exception = Assert.Throws<FaultyExecutionOrder>(behavior.Cancel);
		Assert.AreEqual("Did you forget to call .Init()?", exception!.Message);
	}
}
