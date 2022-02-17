#! /bin/sh

# pull images
for image in mcr.microsoft.com/dotnet/aspnet:latest mcr.microsoft.com/dotnet/sdk:latest
do
	docker pull $image
done

# build
docker build --tag eassbhhtgu/networkdiscoveryapi:latest .

# push
docker push eassbhhtgu/networkdiscoveryapi:latest

# deploy
docker stack deploy --compose-file ./docker-compose.yml networkdiscoveryapi
