FROM microsoft/dotnet:2.2-sdk
RUN apt-get update
RUN apt-get install -y ffmpeg

WORKDIR /code/Streaming.Api

# We are doing it to not force to run always dotnet restore even on small change in source files
	ADD Streaming.Api/Streaming.Api.csproj /code/Streaming.Api/Streaming.Api.csproj
	ADD Streaming.Application/Streaming.Application.csproj /code/Streaming.Application/Streaming.Application.csproj
	ADD Streaming.Common/Streaming.Common.csproj /code/Streaming.Common/Streaming.Common.csproj
	ADD Streaming.Domain/Streaming.Domain.csproj /code/Streaming.Domain/Streaming.Domain.csproj
	ADD Streaming.Auth0/Streaming.Auth0.csproj /code/Streaming.Auth0/Streaming.Auth0.csproj
	ADD Streaming.Infrastructure/Streaming.Infrastructure.csproj /code/Streaming.Infrastructure/Streaming.Infrastructure.csproj
	RUN dotnet restore
#

ADD Streaming.Api /code/Streaming.Api
ADD Streaming.Application /code/Streaming.Application
ADD Streaming.Common /code/Streaming.Common
ADD Streaming.Domain /code/Streaming.Domain
ADD Streaming.Auth0 /code/Streaming.Auth0
ADD Streaming.Infrastructure /code/Streaming.Infrastructure

CMD dotnet run
