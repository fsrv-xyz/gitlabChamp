FROM mcr.microsoft.com/dotnet/sdk:7.0@sha256:24f22e1142432dea3a34d350686be77be5e454ccb9fab7c00d67cd03c6e000a6 AS build
ARG RELEASE="linux-x64"
ARG VERSION="undefined"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=$VERSION

FROM debian:stable-slim@sha256:c9e860b7a6d51a5f90f4fcb645d789c078f7b9d2cc6ebfd48792e0fea5739c04 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
