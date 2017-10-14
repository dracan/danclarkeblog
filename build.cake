#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.DoInDirectory
#addin nuget:?package=Cake.WebDeploy
#addin Cake.Powershell

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var webDeployPassword = EnvironmentVariable("DanClarkeBlog:WebAzurePassword");
var functionsDeployPassword = EnvironmentVariable("DanClarkeBlog:FunctionsAzurePassword");

Task("NuGetRestoreWeb")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Web", () => {
            DotNetCoreRestore("DanClarkeBlog.Web.csproj");
        });
    });

Task("NuGetRestoreFunctions")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Functions", () => {
            DotNetCoreRestore("DanClarkeBlog.Functions.csproj");
        });
    });

Task("BuildWeb")
    .IsDependentOn("NuGetRestoreWeb")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Web", () => {
            DotNetCoreBuild("DanClarkeBlog.Web.csproj", new DotNetCoreBuildSettings
            {
                Configuration = configuration,
            });
        });
    });

Task("BuildFunctions")
    .IsDependentOn("NuGetRestoreFunctions")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Functions", () => {
            DotNetCoreBuild("DanClarkeBlog.Functions.csproj", new DotNetCoreBuildSettings
            {
                Configuration = configuration,
            });
        });
    });

Task("PublishWeb")
    .IsDependentOn("BuildWeb")
    .Does(() => {
        DotNetCorePublish("./DanClarkeBlog.Web/DanClarkeBlog.Web.csproj", new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts_web/"
        });
    });

Task("DeployWeb")
    .IsDependentOn("PublishWeb")
    .Does(() =>
    {
        var siteName = "danclarkeblog";

        DeployWebsite(new DeploySettings
        {
            SourcePath = "./artifacts_web",
            SiteName = siteName,
            ComputerName = "https://" + siteName + ".scm.azurewebsites.net:443/msdeploy.axd?site=" + siteName,
            Username = "$" + siteName,
            Password = webDeployPassword,
       });
    });

Task("DeployFunctions")
    .IsDependentOn("BuildFunctions")
    .Does(() =>
    {
        var siteName = "danclarkeblogfunctions";

        DeployWebsite(new DeploySettings
        {
            SourcePath = "./DanClarkeBlog.Functions/bin/Release/net462",
            SiteName = siteName,
            ComputerName = "https://" + siteName + ".scm.azurewebsites.net:443/msdeploy.axd?site=" + siteName,
            Username = "$" + siteName,
            Password = functionsDeployPassword,
       });
    });

Task("Default")
    .IsDependentOn("DeployWeb")
    .IsDependentOn("DeployFunctions")
    .Does(() => {
    });

RunTarget(target);
