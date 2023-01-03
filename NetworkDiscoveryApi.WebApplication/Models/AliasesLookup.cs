using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication.Models;

public class AliasesLookup : 
	Dictionary<string, string>, 
	IReadOnlyDictionary<string, PhysicalAddress>
{
	private readonly Func<string, PhysicalAddress> _parser = PhysicalAddress.Parse;

	public AliasesLookup() : base(StringComparer.OrdinalIgnoreCase) { }

	public new PhysicalAddress this[string key] => _parser(base[key]);
	public new IEnumerable<string> Keys => base.Keys;
	public new IEnumerable<PhysicalAddress> Values => base.Values.Select(_parser);

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out PhysicalAddress value)
	{
		if (base.TryGetValue(key, out var r))
		{
			value = _parser(r);
			return true;
		}

		value = default;
		return false;
	}

	public new IEnumerator<KeyValuePair<string, PhysicalAddress>> GetEnumerator()
	{
		return base.Keys
			.Select(s => new KeyValuePair<string, PhysicalAddress>(s, this[s]))
			.GetEnumerator();
	}
}
