FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY DanClarkeBlog.Core/. DanClarkeBlog.Core/.
COPY DanClarkeBlog.Tasks/. DanClarkeBlog.Tasks/.
WORKDIR /src/DanClarkeBlog.Tasks/
RUN dotnet restore
RUN dotnet publish -c Release

FROM microsoft/dotnet:2.1-runtime
WORKDIR /app
COPY --from=build /src/DanClarkeBlog.Tasks/bin/Release/netcoreapp2.1/publish .
EXPOSE 80
LABEL version="2.0.9"
ENTRYPOINT dotnet DanClarkeBlog.Tasks.dll
