FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /src
COPY DanClarkeBlog.Core/. DanClarkeBlog.Core/.
COPY DanClarkeBlog.Web/. DanClarkeBlog.Web/.
WORKDIR /src/DanClarkeBlog.Web/
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ARG VersionNumber="0.0.0"
LABEL version=${VersionNumber}
ENV Blog__VersionNumber=${VersionNumber}
COPY --from=build /src/DanClarkeBlog.Web/bin/Release/netcoreapp2.2/publish .
EXPOSE 80
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
