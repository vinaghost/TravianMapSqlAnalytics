FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY bin/MapSqlAspNetCoreMVC/ ./
RUN ["chmod", "+x", "MapSqlAspNetCoreMVC"]
ENTRYPOINT ["./MapSqlAspNetCoreMVC"]