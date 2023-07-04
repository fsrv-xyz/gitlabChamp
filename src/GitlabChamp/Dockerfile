FROM mcr.microsoft.com/dotnet/sdk:7.0@sha256:79cacf6458f5ffb9c6af0659a7a651d5217ca4f7325db15b1cae01f6712e8979 AS build
ARG RELEASE="linux-x64"
ARG VERSION="undefined"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=$VERSION

FROM debian:stable-slim@sha256:bfb338bbc84031a32bb0d4ba61025728ac725968b562707716c0fcc42775d26b as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/GitlabChamp"]
