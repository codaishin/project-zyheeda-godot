using Godot;
using NUnit.Framework;


public class TestCollection {
	[TearDown]
	public void CleanNodes() {
		var baseNode = Tests.BaseNode;
		foreach (var child in baseNode.GetChildren()) {
			if (child is Node node) {
				baseNode.RemoveChild(node);
				node.QueueFree();
			}
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
}
