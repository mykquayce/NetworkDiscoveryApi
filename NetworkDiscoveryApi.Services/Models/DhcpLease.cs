using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Models;

public record DhcpLease(DateTime Expiry, PhysicalAddress PhysicalAddress, IPAddress IPAddress, string? HostName, string? Alias);
