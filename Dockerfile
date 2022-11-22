FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG RELEASE="linux-arm64"
WORKDIR /source
COPY . .
RUN dotnet publish -c Release -r $RELEASE -o /app

FROM debian:stable-slim as runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENTRYPOINT ["/app/gitlabChamp"]
CMD ["--urls", "http://0.0.0.0:5000"]
