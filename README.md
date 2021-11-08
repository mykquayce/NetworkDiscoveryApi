# Network Discovery API
A simple API to query your router's DHCP data.  It gets the data by reading the `/tmp/dhcp.leases` file via SSH.

5 configs:
1. `Router:Host` (defaults to `localhost`)
1. `Router:Port` (defaults to `22`)
1. `Router:Username` (defaults to `root`)
1. `Router:Password` (defaults to blank)
1. `Router:PathToPrivateKey` (defaults to blank)

These can be set in the `appsettings.json` or user secrets with ID `ed0c99a4-1a72-46ee-be0b-6051c416da5a`...
```bash
dotnet user-secrets set Router:Host localhost --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Router:Port 22 --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Router:Username root --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Router:Password ... --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Router:PathToPrivateKey ... --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
```
## Tests
`NetworkDiscoveryApi.WebApplication.Tests` loads its config from user secrets ID `1c40e017-4811-4ef9-b07e-1f3409df0453`
