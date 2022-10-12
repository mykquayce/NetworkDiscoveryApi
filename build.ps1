# pull images
$images = @(
	"mcr.microsoft.com/dotnet/aspnet:7.0",
	"mcr.microsoft.com/dotnet/sdk:7.0")

foreach ($image in $images) {
	docker pull $image
	if (!$?) { return; }
}

# build
docker build `
	--secret id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt `
	--tag eassbhhtgu/networkdiscoveryapi:latest `
	.
if (!$?) { return; }

# push
docker push eassbhhtgu/networkdiscoveryapi:latest
if (!$?) { return; }
