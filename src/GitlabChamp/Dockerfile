FROM mcr.microsoft.com/dotnet/sdk:7.0@sha256:1015914ba9e3d3ab350f522aa446d17d7ee68190e431a25c0c11e678081327c7 AS build
ARG RELEASE="linux-x64"
ARG VERSION="undefined"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=$VERSION

FROM debian:stable-slim@sha256:b09f68bffcf9c14f3105f262e92321d05abaf48460d1f43f884325bcd4395b95 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
