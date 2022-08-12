using Godot;
using NUnit.Framework;

[TestFixture]
public class NodeExtensionsTests : TestCollection {
	[Test]
	public void GetChildrenGenericEmpty() {
		var node = new Node();
		Tests.BaseNode.AddChild(node);

		Assert.IsEmpty(node.GetChildren<Node>());
	}

	[Test]
	public void GetChildrenGeneric() {
		var node = new Node();
		var child = new Node();
		Tests.BaseNode.AddChild(node);
		node.AddChild(child);

		CollectionAssert.AreEquivalent(new[] { child }, node.GetChildren<Node>());
	}

	[Test]
	public void GetChildrenGenericEmptyOnTypeMismatch() {
		var node = new Node();
		var child = new KinematicBody();
		Tests.BaseNode.AddChild(node);
		node.AddChild(child);

		Assert.IsEmpty(node.GetChildren<Timer>());
	}
}
