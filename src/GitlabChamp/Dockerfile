FROM mcr.microsoft.com/dotnet/sdk:7.0@sha256:746fb86d08ca172ae0880c585ea86d8682e297fca5e5ff269ebd31d793d050d3 AS build
ARG RELEASE="linux-x64"
ARG VERSION="undefined"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=$VERSION

FROM debian:stable-slim@sha256:6fe30b9cb71d604a872557be086c74f95451fecd939d72afe3cffca3d9e60607 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
