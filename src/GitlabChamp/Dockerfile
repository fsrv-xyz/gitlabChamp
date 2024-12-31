FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:5f21ebd358442f40099c997a3f4db906a7b1bd872249e67559f55de654b55d3b AS runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
