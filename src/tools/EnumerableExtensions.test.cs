using NUnit.Framework;

[TestFixture]
public class EnumerableExtensionsTests {
	[Test]
	public void JoinOneString() {
		var value = new[] { "hello" };
		Assert.AreEqual("hello", value.Join(""));
	}

	[Test]
	public void JoinEmptyArray() {
		var value = new string[0];
		Assert.AreEqual("", value.Join(""));
	}

	[Test]
	public void Join5StringsWithColon() {
		var value = new[] { "aa", "bb", "cc", "dd", "ee" };
		Assert.AreEqual("aa:bb:cc:dd:ee", value.Join(":"));
	}

	[Test]
	public void Join5StringsWithDoubleHyphen() {
		var value = new[] { "aa", "bb", "cc", "dd", "ee" };
		Assert.AreEqual("aa--bb--cc--dd--ee", value.Join("--"));
	}

	[Test]
	public void Join5NumbersWithColon() {
		var value = new[] { 1, 2, 3, 4, 5 };
		Assert.AreEqual("1:2:3:4:5", value.Join(":"));
	}
}
