# pull images
$images = @(
	"mcr.microsoft.com/dotnet/aspnet:latest",
	"mcr.microsoft.com/dotnet/sdk:latest")

foreach ($image in $images) {
	docker pull $image
	if (!$?) { return; }
}

# build
docker build --tag eassbhhtgu/networkdiscoveryapi:latest .
if (!$?) { return; }

# push
docker push eassbhhtgu/networkdiscoveryapi:latest
