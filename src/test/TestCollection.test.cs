using System.Linq;
using Godot;
using NUnit.Framework;

public class TestCollection {
	[TearDown]
	public void CleanNodes() {
		var root = Tests.BaseNode;
		foreach (var child in root.GetChildren<Node>()) {
			root.RemoveChild(child);
			child.QueueFree();
		}
		root = Tests.BaseNode.GetTree().Root;
		var isNotBaseNode = (Node c) => c != Tests.BaseNode;
		foreach (var child in root.GetChildren<Node>().Where(isNotBaseNode)) {
			root.RemoveChild(child);
			child.QueueFree();
		}
	}
}

[TestFixture]
public class TestCollectionTests : TestCollection {
	[Test]
	public void NodesDoNotBleedInOtherTestsA() {
		var node = new Node();
		Tests.BaseNode.AddChild(node);
		CollectionAssert.AreEquivalent(
			new[] { node },
			Tests.BaseNode.GetChildren()
		);
	}

	[Test]
	public void NodesDoNotBleedInOtherTestsB() {
		var node = new Node();
		Tests.BaseNode.AddChild(node);
		CollectionAssert.AreEquivalent(
			new[] { node },
			Tests.BaseNode.GetChildren()
		);
	}

	[Test]
	public void NodesUnderTreeDoNotBleedInOtherTestsA() {
		var node = new Node();
		Tests.BaseNode.GetTree().Root.AddChild(node);
		CollectionAssert.AreEquivalent(
			new[] { node, Tests.BaseNode },
			Tests.BaseNode.GetTree().Root.GetChildren()
		);
	}

	[Test]
	public void NodesUnderTreeDoNotBleedInOtherTestsB() {
		var node = new Node();
		Tests.BaseNode.GetTree().Root.AddChild(node);
		CollectionAssert.AreEquivalent(
			new[] { node, Tests.BaseNode },
			Tests.BaseNode.GetTree().Root.GetChildren()
		);
	}
}
