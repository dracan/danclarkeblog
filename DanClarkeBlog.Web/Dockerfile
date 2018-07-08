FROM microsoft/dotnet:2.1-aspnetcore-runtime

ARG source=./bin/Release/netcoreapp2.1/publish
WORKDIR /app
EXPOSE 80
LABEL version="2.0.5"
COPY $source .
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
