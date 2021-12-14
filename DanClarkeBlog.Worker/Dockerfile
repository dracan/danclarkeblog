FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src

COPY DanClarkeBlog.Core/*.csproj DanClarkeBlog.Core/
COPY DanClarkeBlog.Worker/*.csproj DanClarkeBlog.Worker/
WORKDIR /src/DanClarkeBlog.Worker
RUN dotnet restore

COPY DanClarkeBlog.Core/. /src/DanClarkeBlog.Core/.
COPY DanClarkeBlog.Worker/. /src/DanClarkeBlog.Worker/.
WORKDIR /src/DanClarkeBlog.Worker/
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
ARG VersionNumber="0.0.0"
LABEL version=${VersionNumber}
ENV Blog__VersionNumber=${VersionNumber}
COPY --from=build /src/DanClarkeBlog.Worker/bin/Release/net6.0/publish .
EXPOSE 80
ENTRYPOINT dotnet DanClarkeBlog.Worker.dll
