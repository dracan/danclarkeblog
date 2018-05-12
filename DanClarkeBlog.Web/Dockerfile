FROM microsoft/aspnetcore
ARG source=./bin/Release/netcoreapp2.0/publish
WORKDIR /app
EXPOSE 80
LABEL version="2.0.1"
COPY $source .
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
