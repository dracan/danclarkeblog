FROM microsoft/dotnet:2.1-sdk
WORKDIR /src/DanClarkeBlog.Web
EXPOSE 80
ENTRYPOINT ["dotnet", "run"]
