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

mkdir /tmp/rec
chmod u+x /root/makeRequest.sh
chmod u+x /root/runMachine.sh
