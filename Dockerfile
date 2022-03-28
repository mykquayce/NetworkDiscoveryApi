FROM eassbhhtgu/certs:latest as certs

FROM mcr.microsoft.com/dotnet/sdk:latest as build-env
WORKDIR /app
COPY . .
COPY --from=certs /usr/local/share/ca-certificates/ca.crt /usr/local/share/ca-certificates/ca.crt
RUN /usr/sbin/update-ca-certificates
RUN dotnet restore --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet publish --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:latest
EXPOSE 443/tcp
ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "NetworkDiscoveryApi.WebApplication.dll"]
