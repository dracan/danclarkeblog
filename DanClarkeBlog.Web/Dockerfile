FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
WORKDIR /src
COPY DanClarkeBlog.Core/. DanClarkeBlog.Core/.
COPY DanClarkeBlog.Web/. DanClarkeBlog.Web/.
WORKDIR /src/DanClarkeBlog.Web/
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
ARG VersionNumber="0.0.0"
LABEL version=${VersionNumber}
ENV Blog__VersionNumber=${VersionNumber}
COPY --from=build /src/DanClarkeBlog.Web/bin/Release/netcoreapp3.0/publish .
EXPOSE 80
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
