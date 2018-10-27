FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY DanClarkeBlog.Core/. DanClarkeBlog.Core/.
COPY DanClarkeBlog.Web/. DanClarkeBlog.Web/.
WORKDIR /src/DanClarkeBlog.Web/
RUN dotnet restore
RUN dotnet publish -c Release

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
ARG VersionNumber="0.0.0"
LABEL version=${VersionNumber}
ENV Blog__VersionNumber=${VersionNumber}
COPY --from=build /src/DanClarkeBlog.Web/bin/Release/netcoreapp2.1/publish .
EXPOSE 80
ENTRYPOINT dotnet DanClarkeBlog.Web.dll
