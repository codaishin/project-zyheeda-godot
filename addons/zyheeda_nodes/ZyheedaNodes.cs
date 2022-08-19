#if TOOLS
using Godot;

[Tool]
public class ZyheedaNodes : EditorPlugin {

	private static readonly (string, string)[] nodes = new[]{
		("BehaviorNode", "Node"),
	};

	public override void _EnterTree() {
		foreach (var (node, @base) in ZyheedaNodes.nodes) {
			this.AddCustomType(
				node,
				@base,
				GD.Load<Script>($"src/scripts/{node}.cs"),
				GD.Load<Texture>("addons/zyheeda_nodes/node_icon.png")
			);
		}
	}

	public override void _ExitTree() {
		foreach (var (node, _) in ZyheedaNodes.nodes) {
			this.RemoveCustomType(node);
		}
	}
}
#endif
