using Godot;

public interface IEquipment {
	IBehaviorStateMachine GetBehaviorsFor(Node agent);
}
