FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:07414cffa075d67d97a26ad13b0838d9ecb0e22df3384667881ac7a741c472aa AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:0941f9e9cc96c4106845a381fb6fca98393f5f659f3eba6a64e9f79219165cfc as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
