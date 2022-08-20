using System;

public static class MaybeExtensions {
	public static Maybe<TOut> Map<TIn, TOut>(
		this Maybe<TIn> value,
		Func<TIn, TOut> func
	) where TIn : notnull
		where TOut : notnull {

		Maybe<TOut> returnValue = new Maybe<TOut>.None();
		value.Match(
			some: v => returnValue = new Maybe<TOut>.Some(func(v)),
			none: () => returnValue = new Maybe<TOut>.None()
		);

		return returnValue;
	}

	public static Maybe<TOut> Bind<TIn, TOut>(
		this Maybe<TIn> value,
		Func<TIn, Maybe<TOut>> func
	) where TIn : notnull
		where TOut : notnull {

		Maybe<TOut> returnValue = new Maybe<TOut>.None();
		value.Match(
			some: v => returnValue = func(v),
			none: () => returnValue = new Maybe<TOut>.None()
		);
		return returnValue;
	}
}
