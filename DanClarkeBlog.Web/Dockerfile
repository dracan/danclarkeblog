FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src

COPY DanClarkeBlog.Core/*.csproj DanClarkeBlog.Core/
COPY DanClarkeBlog.Web/*.csproj DanClarkeBlog.Web/
WORKDIR /src/DanClarkeBlog.Web/
RUN dotnet restore

COPY DanClarkeBlog.Core/. /src/DanClarkeBlog.Core/.
COPY DanClarkeBlog.Web/. /src/DanClarkeBlog.Web/.
WORKDIR /src/DanClarkeBlog.Web/
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
ARG VersionNumber="0.0.0"
LABEL version=${VersionNumber}
ENV Blog__VersionNumber=${VersionNumber}
COPY --from=build /src/DanClarkeBlog.Web/bin/Release/net6.0/publish .
EXPOSE 80
ENV Logging__Console__FormatterName=
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
