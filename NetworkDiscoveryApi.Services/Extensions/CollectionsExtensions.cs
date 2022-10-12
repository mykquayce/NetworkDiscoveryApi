namespace System.Collections.Generic;

public static class CollectionsExtensions
{
	public static IDictionary<TValue, ICollection<TKey>> Invert<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		where TValue : notnull
	{
		var result = new Dictionary<TValue, ICollection<TKey>>();

		foreach (var (key, value) in dictionary)
		{
			if (result.TryGetValue(value, out ICollection<TKey>? keys))
			{
				keys.Add(key);
			}
			else
			{
				result.Add(value, new List<TKey> { key, });
			}
		}

		return result;
	}
}
