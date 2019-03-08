FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /src
COPY DanClarkeBlog.Core/. DanClarkeBlog.Core/.
COPY DanClarkeBlog.Tasks/. DanClarkeBlog.Tasks/.
WORKDIR /src/DanClarkeBlog.Tasks/
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build /src/DanClarkeBlog.Tasks/bin/Release/netcoreapp2.2/publish .
EXPOSE 80
LABEL version="2.0.11"
ENTRYPOINT dotnet DanClarkeBlog.Tasks.dll
