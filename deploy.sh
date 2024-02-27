#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

CURRENTPATH=$(pwd)
VERSION=$(cat src/Majorsilence.Media.Web/Majorsilence.Media.Web.csproj | grep "<Version>" | sed 's/[^0-9.]*//g')

echo "Deploying version $VERSION"

USER=$2
PASSWORD=$3
SERVER=$4

publish_server() {
	echo "deploy docker media_web"
	scp "$CURRENTPATH/build/media_web.tar.gz" $USER@$SERVER:/root

	ssh $USER@$SERVER '/snap/bin/docker stop media_web || true && /snap/bin/docker rm media_web || true \
	&& zcat /root/media_web.tar.gz | /snap/bin/docker load \
	&& /snap/bin/docker run -d -p 32001:8080 -v /root:/mediadata --restart always --name media_web majorsilence/media_web'

	echo "deploy docker media_workerservice"
	scp "$CURRENTPATH/build/media_workerservice.tar.gz" $USER@$SERVER:/root

	ssh $USER@$SERVER '/snap/bin/docker stop media_workerservice || true && /snap/bin/docker rm media_workerservice || true \
	&& zcat /root/media_workerservice.tar.gz | /snap/bin/docker load \
	&& /snap/bin/docker run -d -v /root:/mediadata --restart always --name media_workerservice majorsilence/media_workerservice'
}

remote_systemd_deploy() {
	echo "deploy systemd media-web"
	sshpass -p "$PASSWORD" ssh -o StrictHostKeyChecking=no -l $USER $SERVER "cd /home/$USER && rm -rf ./media-web-*"

	sshpass -p "$PASSWORD" scp "$CURRENTPATH/build/linux-x64/media-web-linux-x64-$VERSION.tar.gz" $USER@$SERVER:/home/$USER

	sshpass -p "$PASSWORD" ssh -o StrictHostKeyChecking=no -l $USER $SERVER "cd /home/$USER \
	&& tar xvf media-web-linux-x64-$VERSION.tar.gz \
	&& echo $PASSWORD | sudo -S rm -rf /opt/majorsilence/media-web/app \
	&& echo $PASSWORD | sudo -S mkdir -p /opt/majorsilence/media-web/app \
	&& echo $PASSWORD | sudo -S mkdir -p /opt/majorsilence/media-web/data \
	&& echo $PASSWORD | sudo -S cp --recursive /home/$USER/media-web-linux-x64-$VERSION/* /opt/majorsilence/media-web/app \
	&& rm -rf /home/$USER/media-web-* \
	&& echo $PASSWORD | sudo -S chown media-web:media-web -R /opt/majorsilence/media-web \
	&& echo $PASSWORD | sudo -S chmod +x /opt/majorsilence/media-web/app \
	&& echo $PASSWORD | sudo -S systemctl restart majorsilence-media-web"

	echo "deploy systemd media-workerservice"
	sshpass -p "$PASSWORD" ssh -o StrictHostKeyChecking=no -l $USER $SERVER "cd /home/$USER && rm -rf ./media-workerservice-*"

	sshpass -p "$PASSWORD" scp "$CURRENTPATH/build/linux-x64/media-workerservice-linux-x64-$VERSION.tar.gz" $USER@$SERVER:/home/$USER

	sshpass -p "$PASSWORD" ssh -o StrictHostKeyChecking=no -l $USER $SERVER "cd /home/$USER \
	&& tar xvf media-workerservice-linux-x64-$VERSION.tar.gz \
	&& echo $PASSWORD | sudo -S rm -rf /opt/majorsilence/media-workerservice/app \
	&& echo $PASSWORD | sudo -S mkdir -p /opt/majorsilence/media-workerservice/app \
	&& echo $PASSWORD | sudo -S mkdir -p /opt/majorsilence/media-workerservice/data \
	&& echo $PASSWORD | sudo -S cp --recursive /home/$USER/media-workerservice-linux-x64-$VERSION/* /opt/majorsilence/media-workerservice/app \
	&& rm -rf /home/$USER/media-workerservice-* \
	&& echo $PASSWORD | sudo -S chown media-workerservice:media-workerservice -R /opt/majorsilence/media-workerservice \
	&& echo $PASSWORD | sudo -S chmod +x /opt/majorsilence/media-workerservice/app \
	&& echo $PASSWORD | sudo -S systemctl restart majorsilence-media-workerservice"
}

if [ "$1" = "remote_docker_deploy" ]; then
	publish_server
elif [ "$1" = "remote_systemd_deploy" ]; then
	remote_systemd_deploy
elif [ "$1" = "local_systemd_deploy" ]; then
	local_systemd_deploy
else
	echo "Only \"local_docker_install\", \"remote_docker_deploy\", \"remote_systemd_deploy\", or \"local_systemd_deploy\" supported."
fi
