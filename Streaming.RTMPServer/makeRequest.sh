#!/bin/bash
echo `whoami` > /tmp/rec/test.txt
echo `echo $UPLOAD_ENDPOINT` > /tmp/rec/test2.txt
echo `printenv` > /tmp/rec/test3.txt
curl -F "file=@$1" `echo $UPLOAD_ENDPOINT`
rm $1