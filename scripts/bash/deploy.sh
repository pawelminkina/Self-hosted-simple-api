#!/usr/bin/env bash
#random change, deployh
git pull
cd src

BUILD_VERSION=$(git rev-parse HEAD)

echo "$(date --utc +%FT%TZ): Releasing new server version. $BUILD_VERSION"

echo "$(date --utc +%FT%TZ): Running build..."
docker compose rm -f
docker compose build

OLD_CONTAINER=$(docker ps -aqf "name=simpleapi")
echo "$(date --utc +%FT%TZ): Scaling server up..."
BUILD_VERSION=$BUILD_VERSION docker compose up -d --scale simpleapi=2 --no-deps --no-recreate simpleapi

sleep 30

echo "$(date --utc +%FT%TZ): Scaling old server down..."
docker container rm -f $OLD_CONTAINER
docker compose up -d --scale simpleapi=1 --no-deps --no-recreate simpleapi

echo "$(date --utc +%FT%TZ): Reloading caddy..."
CADDY_CONTAINER=$(docker ps -aqf "name=caddy")
docker exec $CADDY_CONTAINER caddy reload -c /etc/caddy/Caddyfile