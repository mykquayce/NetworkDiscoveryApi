FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates

WORKDIR /app
COPY . .
RUN dotnet restore --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet publish --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 443/tcp
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "NetworkDiscoveryApi.WebApplication.dll"]
