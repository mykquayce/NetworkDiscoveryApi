docker pull eassbhhtgu/networkdiscoveryapi:latest
if (!$?) { return; }

docker stack deploy --compose-file .\docker-compose.yml networkdiscovery
