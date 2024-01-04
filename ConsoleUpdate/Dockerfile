FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY bin/ConsoleUpdate/ ./
RUN ["chmod", "+x", "ConsoleUpdate"]
ENTRYPOINT ["./ConsoleUpdate"]