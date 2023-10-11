FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:c3d084185dee85aa204797dd228e3e720a14dbcfe4e785b40b9df189f79c7190 AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:8cfbea7c925801b678943e59e3ae6fc5b925e8bfe0243f4e724e180edb5aabcd as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
