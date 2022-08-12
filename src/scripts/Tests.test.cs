using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

[TestFixture]
public class TestsTests : TestCollection {

	private class ProcessCountNode : Node {
		public int processCount = 0;

		public override void _PhysicsProcess(float delta) {
			++this.processCount;
		}
	}

	[Test, Timeout(2000)]
	public async Task RunOverSeveralFrames() {
		var tree = Tests.BaseNode.GetTree();
		var node = new ProcessCountNode();
		Tests.BaseNode.AddChild(node);
		_ = await tree.ToSignal(tree, "physics_frame");
		_ = await tree.ToSignal(tree, "physics_frame");
		_ = await tree.ToSignal(tree, "physics_frame");
		Assert.AreEqual(2, node.processCount);
	}
}
