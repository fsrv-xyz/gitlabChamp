FROM mcr.microsoft.com/dotnet/sdk:8.0.100@sha256:239eb99884b49bac13f4ff5935e118654f87b67d6e31bc842c7cd5a6c46fcc4f AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:4255c9f8a4d6e66488adc0c2084c99df44bda22849b21b3afc0e9746e9a0be18 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
