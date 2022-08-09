using System.Collections.Generic;

public static class EnumerableExtensions {
	public static string Join<T>(this IEnumerable<T> value, string delim) {
		return string.Join(delim, value);
	}
}
