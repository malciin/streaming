#!/bin/bash
curl -F "file=@$1" `echo $UPLOAD_ENDPOINT`
rm $1