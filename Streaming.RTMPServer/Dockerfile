FROM ubuntu:16.04

RUN apt-get update

# Basic utilities
RUN apt-get install -y build-essential python
RUN apt-get install -y apt-utils net-tools curl wget iputils-ping lsof
RUN apt-get install -y git
RUN apt-get install -y vim
RUN apt-get install -y ffmpeg
RUN apt-get install -y sudo
RUN apt-get install -y unzip # needed for ./configure

WORKDIR /root
RUN wget ny.malcin.tk/rtmp-1554068040.tgz
RUN sudo tar -xvf rtmp-1554068040.tgz
WORKDIR /root/trunk

RUN ./configure && make

EXPOSE 1935
EXPOSE 80

COPY srs.conf srs.conf
CMD ./objs/srs -c srs.conf && tail -f ./objs/srs.log -f localServer.log
