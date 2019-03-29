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
            live on;

            # on_publish [PUBLISH_ENDPOINT];
            # on_done [STOP_PUBLISH_ENDPOINT];
            # exec_record_done /root/makeRequest.sh \$path;

            # record all;
            # record_path /tmp/rec;
            # record_unique on;
			# record_max_size 3000K;
			# exec_record_done ffmpeg -y -i $path -acodec libmp3lame -ar 44100 -ac 1 -vcodec libx264 $dirname/$basename.mp4;
            # record_interval 10s;
			
			hls on;
			hls_path /tmp/rec;
			hls_fragment 3s;
			hls_nested on;

			# exec_push echo 'test.txt' >> /tmp/rec/test.txt;

            # disable consuming the stream from nginx as rtmp
            deny play all;
        }
    }
}
" >> /usr/local/nginx/conf/nginx.conf

mkdir /tmp/rec
chmod u+x /root/makeRequest.sh
chmod u+x /root/runMachine.sh
