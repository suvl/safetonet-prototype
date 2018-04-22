FROM microsoft/dotnet:2.1-sdk AS builder

# Install nodejs and react-scripts
RUN apt-get update && apt-get install -y apt-utils && \
    curl -sL https://deb.nodesource.com/setup_9.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g react-scripts

COPY . /src
WORKDIR /src/SafeToNet.Prototype.Api
RUN dotnet publish -o publish/


FROM microsoft/dotnet:2.1-aspnetcore-runtime

RUN apt-get update && apt-get full-upgrade -y && apt-get install -y gnupg && \
    curl -sL https://deb.nodesource.com/setup_9.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g react-scripts serve && \
    apt-get install -y nginx

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://+:5000
ENV PORT 3000

COPY --from=builder /src/SafeToNet.Prototype.Api/publish /app
COPY nginx.conf /etc/nginx/nginx.conf
COPY start.sh /app/start.sh

WORKDIR /app

CMD ["/bin/bash","start.sh"]
