using System.Collections.Generic;

namespace LWC
{
    public static class DictionaryExtension
	{
		
		public static TValue Get<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TKey key)
		{
			TValue value = default(TValue);
			dictionary.TryGetValue(key, out value);
			return value;
		}
		
	}
}
