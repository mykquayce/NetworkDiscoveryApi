#! /bin/bash

for image in \
	mcr.microsoft.com/dotnet/aspnet:6.0 \
	mcr.microsoft.com/dotnet/sdk:6.0
do
	docker pull $image
done

docker build --tag eassbhhtgu/networkdiscoveryapi:latest .

docker stack ls | tail --line +2 | findstr networkdiscoveryapi

if [ $? -ne 0 ]; then
	# create one
	docker stack deploy --compose-file ./docker-compose.yml 
else
	# update it
	docker service ls | \
		tail --line +2 | \
		findstr networkdiscoveryapi | \
		awk '{system("docker service update --image " $5 " " $2)}'
fi
