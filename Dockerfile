FROM microsoft/dotnet:2.2-sdk
ADD Streaming.Api /code/Streaming.Api
ADD Streaming.Application /code/Streaming.Application
ADD Streaming.Common /code/Streaming.Common
ADD Streaming.Domain /code/Streaming.Domain
WORKDIR /code/Streaming.Api
RUN apt-get update
RUN apt-get install -y ffmpeg
RUN dotnet restore
CMD dotnet run
