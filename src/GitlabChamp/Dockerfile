FROM mcr.microsoft.com/dotnet/sdk:8.0.101@sha256:8e77ad6fb7c33c17f026424d3bef05ea2ee15d1621e28f312adeab4dc1005866 AS build
ARG RELEASE="linux-x64"
ARG REPO_VERSION=${REPO_VERSION:-0.0.0}
RUN echo "Building version ${REPO_VERSION} for ${RELEASE}"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=${REPO_VERSION#v}

FROM debian:stable-slim@sha256:0200978f5b28cc795ec1699254fd789263193af9ab650bd2e4ef2bedf6cbd1c2 as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
