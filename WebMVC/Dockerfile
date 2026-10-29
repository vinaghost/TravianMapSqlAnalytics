FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /source

COPY Features/Features.csproj Features/
COPY Features/packages.lock.json Features/

COPY WebMVC/WebMVC.csproj WebMVC/
COPY WebMVC/packages.lock.json WebMVC/

RUN dotnet restore WebMVC/WebMVC.csproj /p:RestorePackagesWithLockFile=true /p:PublishReadyToRun=true -a $TARGETARCH 

COPY Features/ Features/
COPY WebMVC/ WebMVC/
RUN dotnet publish WebMVC/WebMVC.csproj /p:PublishReadyToRun=true -a $TARGETARCH --self-contained --no-restore -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS final
EXPOSE 8080
WORKDIR /app
COPY --from=build --chown=app:app /app .
ENTRYPOINT ["./WebMVC"]