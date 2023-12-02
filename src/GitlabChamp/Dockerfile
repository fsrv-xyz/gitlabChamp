FROM mcr.microsoft.com/dotnet/sdk:8.0.100@sha256:239eb99884b49bac13f4ff5935e118654f87b67d6e31bc842c7cd5a6c46fcc4f AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:375fb84f3c64691a1b9a9ff5ff3905173dcd0c5e11bc2aebd5c3472a139fa2b4 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
