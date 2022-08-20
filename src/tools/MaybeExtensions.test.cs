using NUnit.Framework;


[TestFixture]
public class NullableExtensionsTests : TestCollection {
	[Test]
	public void MapSome() {
		var value = new Maybe<int>.Some(42);
		var result = value.Map(v => v.ToString());

		result.Match(
			some: v => Assert.AreEqual("42", v),
			none: () => Assert.Fail("Expected Some, was None")
		);
	}

	[Test]
	public void MapNone() {
		var value = new Maybe<object>.None();
		var result = value.Map(v => v.ToString());
		Assert.IsInstanceOf<Maybe<string>.None>(result);
	}

	[Test]
	public void BindSome() {
		var value = new Maybe<int>.Some(42);
		var result = value.Bind(_ => new Maybe<float>.Some(4.2f));

		result.Match(
			some: v => Assert.AreEqual(4.2f, v),
			none: () => Assert.Fail("Expected Some, was None")
		);
	}

	[Test]
	public void BindNone() {
		var value = new Maybe<object>.None();
		var result = value.Bind(v => new Maybe<string>.None());
		Assert.IsInstanceOf<Maybe<string>.None>(result);
	}
}
