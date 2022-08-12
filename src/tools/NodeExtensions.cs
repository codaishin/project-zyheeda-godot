using System.Collections.Generic;
using Godot;

public static class NodeExtensions {
	public static IEnumerable<TNode> GetChildren<TNode>(this Node node)
		where TNode : Node {
		foreach (var child in node.GetChildren()) {
			if (child is TNode childNode) {
				yield return childNode;
			}
		}
	}
}
