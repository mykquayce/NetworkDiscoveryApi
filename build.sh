#! /bin/bash

# pull images used by Dockerfile
cat ./Dockerfile | grep --ignore-case ^from | awk '{system("docker pull " $2)}'

# pull images used by docker-compose.yml
cat ./docker-compose.yml | grep --ignore-case image: | awk '{system("docker pull " $2)}'

# build
docker build --tag eassbhhtgu/networkdiscoveryapi:latest .

# push
docker push eassbhhtgu/networkdiscoveryapi:latest

# is there a stack running?
docker stack ls | tail --line +2 | findstr networkdiscoveryapi

if [ $? -ne 0 ]; then
	# deploy one
	docker stack deploy --compose-file ./docker-compose.yml networkdiscoveryapi
else
	# update the service
	docker service ls | \
		tail --line +2 | \
		findstr networkdiscoveryapi | \
		awk '{system("docker service update --image " $5 " " $2)}'
fi
