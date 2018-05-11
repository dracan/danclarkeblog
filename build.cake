#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.DoInDirectory
#addin nuget:?package=Cake.WebDeploy
#addin Cake.Powershell
#addin "Cake.Docker"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

string version;

var configDirectory = EnvironmentVariable("Blog__KubernetesConfigDirectory");

Task("CalculateVersionNumber")
    .Does(() => {
        GitVersion assertedVersions = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
        version = assertedVersions.LegacySemVerPadded;
        Information("Calculated Semantic Version: {0}", version);
    });

Task("UpdateDockerFileVersions")
    .IsDependentOn("CalculateVersionNumber")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Web", () => {
            var data = System.IO.File.ReadAllText("Dockerfile");
            data = System.Text.RegularExpressions.Regex.Replace(data, $@"LABEL version=""\d+\.\d+\.\d+""", $@"LABEL version=""{version}""");
            System.IO.File.WriteAllText("Dockerfile", data);
        });

        DoInDirectory(@"DanClarkeBlog.Tasks", () => {
            var data = System.IO.File.ReadAllText("Dockerfile");
            data = System.Text.RegularExpressions.Regex.Replace(data, $@"LABEL version=""\d+\.\d+\.\d+""", $@"LABEL version=""{version}""");
            System.IO.File.WriteAllText("Dockerfile", data);
        });
    });

Task("UpdateK8sVersions")
    .IsDependentOn("CalculateVersionNumber")
    .Does(() => {
        DoInDirectory(@"Kubernetes", () => {
            var yaml = System.IO.File.ReadAllText("web.yaml");
            yaml = System.Text.RegularExpressions.Regex.Replace(yaml, $@"(image: .*/blog):(\d+\.\d+\.\d+)", $"$1:{version}");
            System.IO.File.WriteAllText("web.yaml", yaml);

            yaml = System.IO.File.ReadAllText("tasks.yaml");
            yaml = System.Text.RegularExpressions.Regex.Replace(yaml, $@"(image: .*/blog-tasks):(\d+\.\d+\.\d+)", $"$1:{version}");
            System.IO.File.WriteAllText("tasks.yaml", yaml);
        });
    });

Task("NuGetRestoreWeb")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Web", () => {
            DotNetCoreRestore("DanClarkeBlog.Web.csproj");
        });
    });

Task("NuGetRestoreTasks")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Tasks", () => {
            DotNetCoreRestore("DanClarkeBlog.Tasks.csproj");
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

Task("BuildTasks")
    .IsDependentOn("NuGetRestoreTasks")
    .Does(() => {
        DoInDirectory(@"DanClarkeBlog.Tasks", () => {
            DotNetCoreBuild("DanClarkeBlog.Tasks.csproj", new DotNetCoreBuildSettings
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
        });
    });

Task("PublishTasks")
    .IsDependentOn("BuildTasks")
    .Does(() => {
        DotNetCorePublish("./DanClarkeBlog.Tasks/DanClarkeBlog.Tasks.csproj", new DotNetCorePublishSettings
        {
            Configuration = configuration,
        });
    });

Task("DockerBuildWeb")
    .IsDependentOn("UpdateDockerFileVersions")
    .IsDependentOn("PublishWeb")
    .Does(() =>
    {
        DockerBuild(new DockerImageBuildSettings { Tag = new[] { $"eu.gcr.io/composed-region-200213/blog:{version}" }, }, "DanClarkeBlog.Web");
    });

Task("DockerBuildTasks")
    .IsDependentOn("UpdateDockerFileVersions")
    .IsDependentOn("PublishTasks")
    .Does(() =>
    {
        DockerBuild(new DockerImageBuildSettings { Tag = new[] { $"eu.gcr.io/composed-region-200213/blog-tasks:{version}" }, }, "DanClarkeBlog.Tasks");
    });

Task("DockerPushWeb")
    .IsDependentOn("CalculateVersionNumber")
    .IsDependentOn("DockerBuildWeb")
    .Does(() =>
    {
        DockerPush($"eu.gcr.io/composed-region-200213/blog:{version}");
    });

Task("DockerPushTasks")
    .IsDependentOn("CalculateVersionNumber")
    .IsDependentOn("DockerBuildTasks")
    .Does(() =>
    {
        DockerPush($"eu.gcr.io/composed-region-200213/blog-tasks:{version}");
    });

Task("K8sApplyConfig")
    .IsDependentOn("UpdateK8sVersions")
    .Does(() =>
    {
        StartProcess("kubectl.exe", $"apply -f {System.IO.Path.Combine(configDirectory, "danclarkeblog-config.yaml")}");
    });

Task("K8sApplyWeb")
    .IsDependentOn("UpdateK8sVersions")
    .IsDependentOn("DockerPushWeb")
    .IsDependentOn("K8sApplyConfig")
    .Does(() =>
    {
        StartProcess("kubectl.exe", "apply -f Kubernetes/web.yaml");
    });

Task("K8sApplyTasks")
    .IsDependentOn("UpdateK8sVersions")
    .IsDependentOn("DockerPushTasks")
    .IsDependentOn("K8sApplyConfig")
    .Does(() =>
    {
        StartProcess("kubectl.exe", "apply -f Kubernetes/tasks.yaml");
    });

Task("Default")
    .IsDependentOn("K8sApplyWeb")
    .IsDependentOn("K8sApplyTasks")
    .Does(() => {
    });

RunTarget(target);
