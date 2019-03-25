FROM ubuntu:18.04
RUN apt-get update

# Basic utilities
RUN apt-get install -y build-essential
RUN apt-get install -y apt-utils
RUN apt-get install -y git
RUN apt-get install -y curl wget iputils-ping lsof
RUN apt-get install -y vim

# RTMP Nginx Server
EXPOSE 1935
RUN apt-get install -y libpcre3 libpcre3-dev libssl-dev zlib1g-dev -y
RUN wget http://nginx.org/download/nginx-1.14.2.tar.gz
RUN tar -xf nginx-1.14.2.tar.gz
RUN git clone https://github.com/sergey-dryabzhinsky/nginx-rtmp-module.git
RUN cd nginx-1.14.2 && ./configure --with-http_ssl_module --add-module=../nginx-rtmp-module
RUN cd nginx-1.14.2 && make install

# SSH Server
EXPOSE 22
RUN apt-get install -y sudo openssh-server
ADD makeRequest.sh /root/makeRequest.sh
ADD setupMachine.sh /root/setupMachine.sh
ADD runMachine.sh /root/runMachine.sh
RUN chmod u+x /root/setupMachine.sh
RUN bash /root/setupMachine.sh
CMD bash /root/runMachine.sh