#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

CURRENTPATH=`pwd`
VERSION=`cat src/Majorsilence.Media.Web/Majorsilence.Media.Web.csproj | grep "<Version>"  | sed 's/[^0-9.]*//g'`

clean_build(){
	rm -rf ./build/docker-images

	# Delete "bin" and "obj" folders
	rm -rf `find -type d -name bin`
	rm -rf `find -type d -name obj`
}

docker_build()
{
	mkdir -p build/docker-images
	GITHASH="$(git rev-parse --short HEAD)"
	docker build -f ./src/Majorsilence.Media.Web/Majorsilence.Media.Web/Dockerfile -t majorsilence/media_web$GITHASH --rm=true .
	docker save majorsilence/media_web$GITHASH | gzip > ./build/docker-images/media_web.tar.gz
	docker rmi majorsilence/media_web$GITHASH

	docker build -f ./src/Majorsilence.Media.Web/Majorsilence.Media.Web/Dockerfile -t majorsilence/media_workerservice$GITHASH --rm=true .
	docker save majorsilence/media_workerservice$GITHASH | gzip > ./build/docker-images/media_workerservice.tar.gz
	docker rmi majorsilence/media_workerservice$GITHASH
}

clean_build
docker_build
