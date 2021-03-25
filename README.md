# NetworkDiscoveryApi

# Docker
```bash
echo -n 192.168.1.10 | docker secret create RouterHost -
echo -n 22 | docker secret create RouterPort -
echo -n root | docker secret create RouterUsername -
echo -n ... | docker secret create RouterPassword -
```

# Web Api
`NetworkDiscoveryApi.WebApplication` user secrets
```bash
dotnet user-secrets set Host 192.168.1.10 --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Port 22 --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Username root --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
dotnet user-secrets set Password ... --id ed0c99a4-1a72-46ee-be0b-6051c416da5a
```

# Tests
`NetworkDiscoveryApi.Services.Tests` has it''s own user secrets id
```bash
dotnet user-secrets set Host 192.168.1.10 --id b04ee387-25ec-4e18-87df-9e75e138c884
dotnet user-secrets set Port 22 --id b04ee387-25ec-4e18-87df-9e75e138c884
dotnet user-secrets set Username root --id b04ee387-25ec-4e18-87df-9e75e138c884
dotnet user-secrets set Password ... --id b04ee387-25ec-4e18-87df-9e75e138c884
```
