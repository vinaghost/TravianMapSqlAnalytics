FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY bin/MapSqlQuery/ ./
RUN ["chmod", "+x", "MapSqlQuery"]
ENTRYPOINT ["./MapSqlQuery"]