FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS prepare-restore-files
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR /source
COPY . .
RUN dotnet subset restore WebAPI/WebAPI.csproj --root-directory /source --output restore_subset/

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
ARG TARGETARCH
WORKDIR /src
COPY --from=prepare-restore-files /source/restore_subset .
RUN dotnet restore WebAPI/WebAPI.csproj --locked-mode -a $TARGETARCH

# copy source code, add more if required other project
COPY WebAPI/ WebAPI/
COPY Application/ Application/
COPY Features/ Features/
COPY Infrastructure/ Infrastructure/
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o /app/publish --no-restore -a $TARGETARCH 

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
RUN ["chmod", "+x", "WebAPI"]
ENTRYPOINT ["./WebAPI"]