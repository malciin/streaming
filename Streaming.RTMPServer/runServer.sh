#!/bin/bash

echo "Writing nginx config..."
sed -i "s/#user.*nobody;/user root;/g" /usr/local/nginx/conf/nginx.conf
echo "
env UPLOAD_ENDPOINT;
error_log logs/error.log debug;
rtmp {
    server {
        listen 1935; # Listen on standard RTMP port
        chunk_size 4000;

        application show {
            on_publish [PUBLISH_ENDPOINT];
            on_done [STOP_PUBLISH_ENDPOINT];
            exec_record_done /root/makeRequest.sh \$path;
            live on;
            record all;
            record_path /tmp/rec;
            record_unique on;
            record_interval 5s;

            # disable consuming the stream from nginx as rtmp
            deny play all;
        }
    }
}
" >> /usr/local/nginx/conf/nginx.conf

if [ -z "$SSH_USERNAME" ] || [ -z "$SSH_PASSWORD" ]
then
	echo '$SSH_USERNAME and/or $SSH_PASSWORD are not setted! The SSH will not be available'
else
	mkdir /var/run/sshd
	sed -ri 's/UsePAM yes/#UsePAM yes/g' /etc/ssh/sshd_config
	sed -i "s/#PasswordAuthentication yes/PasswordAuthentication yes/g" /etc/ssh/sshd_config
	useradd -m `echo $SSH_USERNAME`
	echo "$SSH_USERNAME:$SSH_PASSWORD" | chpasswd
	usermod -aG sudo `echo $SSH_USERNAME`

	echo "Starting openSSH..."
	/usr/sbin/sshd -D &
fi

if [ -z "$PUBLISH_ENDPOINT" ] || [ -z "$STOP_PUBLISH_ENDPOINT" ] || [ -z "$UPLOAD_ENDPOINT" ]
then
    echo 'Please ensure that $PUBLISH_ENDPOINT $STOP_PUBLISH_ENDPOINT $UPLOAD_ENDPOINT env variables are setted'
    exit 1
else
    sed -i "s,\[PUBLISH_ENDPOINT\],`echo $PUBLISH_ENDPOINT`,g" /usr/local/nginx/conf/nginx.conf
    sed -i "s,\[STOP_PUBLISH_ENDPOINT\],`echo $STOP_PUBLISH_ENDPOINT`,g" /usr/local/nginx/conf/nginx.conf
fi


echo "Starting server..."
/usr/local/nginx/sbin/nginx -t
/usr/local/nginx/sbin/nginx -g 'daemon off;'
