using System;

public abstract class Maybe<T> {
	private Maybe() { }
	public abstract void Match(Action<T> some, Action none);

	public class None : Maybe<T> {
		public override void Match(Action<T> some, Action none) {
			none();
		}
	}

	public class Some : Maybe<T> {
		private readonly T value;

		public Some(T value) {
			this.value = value;
		}

		public override void Match(Action<T> some, Action none) {
			some(this.value);
		}
	}
}
