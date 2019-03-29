#!/bin/bash
node localServer.js > localServer.log &
./objs/srs -c srs-custom.conf # Start SRS
tail -f ./objs/srs.log -f localServer.log # Start log monitoring