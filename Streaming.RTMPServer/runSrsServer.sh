#!/bin/bash
sed -i "s,\[ON_CONNECT_ENDPOINT\],$ON_CONNECT_ENDPOINT,g" srs-custom.conf
echo "$ON_CONNECT_ENDPOINT"
cat srs-custom.conf

node localServer.js > localServer.log &
./objs/srs -c srs-custom.conf # Start SRS
tail -f ./objs/srs.log -f localServer.log # Start log monitoring