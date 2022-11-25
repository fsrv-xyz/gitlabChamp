FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG RELEASE="linux-x64"
ARG VERSION="undefined"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app -p:Version=$VERSION

FROM debian:stable-slim as runtime
RUN apt update && apt install -y ca-certificates
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENTRYPOINT ["/app/gitlabChamp"]
CMD ["--urls", "http://0.0.0.0:5000"]
