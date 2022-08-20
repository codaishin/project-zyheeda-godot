using NUnit.Framework;

[TestFixture]
public class MaybeTests : TestCollection {
	[Test]
	public void MatchNone() {
		var value = new Maybe<int>.None();
		value.Match(
			some: _ => Assert.Fail("some should not have been called"),
			none: () => Assert.Pass()
		);
	}
	[Test]
	public void MatchSome() {
		var value = new Maybe<int>.Some(42);
		value.Match(
			some: v => Assert.AreEqual(42, v),
			none: () => Assert.Fail("none should not have been called")
		);
	}
}
