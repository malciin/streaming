#!/bin/bash
OUTPUT=`date +%s%N | cut -b1-13`
ffmpeg -i "$1" -c copy "$1.ts"
curl -F "file=@$1.ts" -F "epochmillis=$OUTPUT" `echo $UPLOAD_ENDPOINT`
# rm "$1" "$1.ts"