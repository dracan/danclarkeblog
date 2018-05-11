FROM microsoft/aspnetcore
ARG source=./bin/Release/netcoreapp2.0/publish
WORKDIR /app
EXPOSE 80
LABEL version="1.0.27"
COPY $source .
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
