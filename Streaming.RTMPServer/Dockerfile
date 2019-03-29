FROM ubuntu:16.04
RUN apt-get update

# Basic utilities
RUN apt-get install -y build-essential python
RUN apt-get install -y apt-utils net-tools curl wget iputils-ping lsof
RUN apt-get install -y git
RUN apt-get install -y vim
RUN apt-get install -y ffmpeg
RUN apt-get install -y sudo

WORKDIR /root
RUN git clone https://github.com/ossrs/srs
WORKDIR /root/srs/trunk
# RUN git checkout 3.0release
RUN ./configure && make
EXPOSE 1935

RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -y nodejs

ADD package.json package.json
ADD localServer.js localServer.js
RUN npm i
ADD srs.conf srs-custom.conf
ADD runSrsServer.sh runSrsServer.sh
RUN chmod u+x runSrsServer.sh
CMD ./runSrsServer.sh