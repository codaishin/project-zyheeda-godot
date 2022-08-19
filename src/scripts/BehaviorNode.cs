using Godot;

public class BehaviorNode : Node {
	[Export] public NodePath? agent;
	[Export] public PackedScene? equipment;

	private IBehaviorStateMachine? states;

	public void Init() {
		var agent = this.GetNode<Node>(this.agent);
		var equipmentNode = this.equipment!.Instance();
		var equipment = (IEquipment)equipmentNode;

		this.AddChild(equipmentNode);

		this.states = equipment.GetBehaviorsFor(agent);
	}

	public void Run() {
		if (this.states is null) {
			throw new FaultyExecutionOrder("Did you forget to call .Init()?");
		}
		this.states.Run();
	}

	public void Cancel() {
		if (this.states is null) {
			throw new FaultyExecutionOrder("Did you forget to call .Init()?");
		}
		this.states!.Reset();
	}
}
