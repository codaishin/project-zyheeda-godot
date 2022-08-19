using System.Linq;
using Godot;
using NUnit.Framework;

public class TestCollection {
	[TearDown]
	public void CleanNodes() {
		var isNotBaseNode = (Node c) => c != Tests.BaseNode;
		var root = Tests.BaseNode;
		var children = root.GetChildren().OfType<Node>();
		foreach (var child in children) {
			root.RemoveChild(child);
			child.QueueFree();
		}
		root = Tests.BaseNode.GetTree().Root;
		children = root.GetChildren().OfType<Node>().Where(isNotBaseNode);
		foreach (var child in children) {
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
