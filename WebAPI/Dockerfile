FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /source

COPY Features/Features.csproj Features/
COPY Features/packages.lock.json Features/

COPY WebAPI/WebAPI.csproj WebAPI/
COPY WebAPI/packages.lock.json WebAPI/

RUN dotnet restore WebAPI/WebAPI.csproj /p:RestorePackagesWithLockFile=true /p:PublishReadyToRun=true -a $TARGETARCH 

COPY Features/ Features/
COPY WebAPI/ WebAPI/
RUN dotnet publish WebAPI/WebAPI.csproj /p:PublishReadyToRun=true -a $TARGETARCH --self-contained --no-restore -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS final
EXPOSE 8080
WORKDIR /app
COPY --from=build --chown=app:app /app .
ENTRYPOINT ["./WebAPI"]