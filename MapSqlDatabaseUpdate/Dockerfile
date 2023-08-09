FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY bin/MapSqlDatabaseUpdate/ ./
RUN ["chmod", "+x", "MapSqlDatabaseUpdate"]
RUN ["ls"]
ENTRYPOINT ["MapSqlDatabaseUpdate"]