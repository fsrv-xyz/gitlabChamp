FROM mcr.microsoft.com/dotnet/sdk:7.0@sha256:926b9337622ccc594ed051d3559f1c4544686c9fd5f4656f09d1fe37d9a5ace2 AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:6fe30b9cb71d604a872557be086c74f95451fecd939d72afe3cffca3d9e60607 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
