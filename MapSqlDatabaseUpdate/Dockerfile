FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY bin/MapSqlDatabaseUpdate/ ./
RUN ["chmod", "+x", "MapSqlDatabaseUpdate"]
ENTRYPOINT ["./MapSqlDatabaseUpdate"]