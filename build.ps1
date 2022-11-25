# pull images
$images = @(
	"mcr.microsoft.com/dotnet/aspnet:7.0",
	"mcr.microsoft.com/dotnet/sdk:7.0")

foreach ($image in $images) {
	docker pull $image
	if (!$?) { return; }
}

# build
$secret = 'id=ca_crt,src={0}\.aspnet\https\ca.crt' -f ${env:userprofile}
docker build `
	--secret $secret `
	--tag eassbhhtgu/networkdiscoveryapi:latest `
	.
if (!$?) { return; }

# push
docker push eassbhhtgu/networkdiscoveryapi:latest
if (!$?) { return; }
