using Dawn;
using System;
using System.Globalization;

namespace NetworkDiscoveryApi.Models
{
	public record DhcpEntry(string? Expiration, string? HostName, string? Identifier, string? IPAddress, string? PhysicalAddress)
	{
		private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
		private const DateTimeStyles _dateTimeStyles = DateTimeStyles.AdjustToUniversal;

		public DhcpEntry() : this(default, default, default, default, default) { }

		public static explicit operator DhcpEntry(Helpers.Networking.Models.DhcpLease other)
		{
			Guard.Argument(() => other).NotNull();
			Guard.Argument(() => other.Expiration).NotDefault();
			Guard.Argument(() => other.IPAddress).NotNull();
			Guard.Argument(() => other.PhysicalAddress).NotNull();

			return new DhcpEntry
			{
				Expiration = other.Expiration.ToString("O", _formatProvider),
				HostName = other.HostName,
				Identifier = other.Identifier,
				IPAddress = other.IPAddress.ToString(),
				PhysicalAddress = other.PhysicalAddress.ToString().ToLowerInvariant(),
			};
		}

		public static explicit operator Helpers.Networking.Models.DhcpLease(DhcpEntry other)
		{
			Guard.Argument(() => other).NotNull();
			Guard.Argument(() => other.Expiration!).NotNull().NotEmpty().NotWhiteSpace().Matches(@"^\d{4}-\d\d-\d\dT\d\d:\d\d:\d\d\.\d{7}Z$");
			Guard.Argument(() => other.PhysicalAddress!).NotNull().NotEmpty().NotWhiteSpace().Matches(@"^[0-9a-f]{12}$");
			Guard.Argument(() => other.IPAddress!).NotNull().NotEmpty().NotWhiteSpace().Matches(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");

			var expiration = DateTime.Parse(other.Expiration!, _formatProvider, _dateTimeStyles);
			var physicalAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(other.PhysicalAddress);
			var ipAddress = System.Net.IPAddress.Parse(other.IPAddress!);

			return new(expiration, physicalAddress, ipAddress, other.HostName, other.Identifier);
		}
	}
}
