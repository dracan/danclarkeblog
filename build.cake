#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.DoInDirectory
#addin nuget:?package=Cake.WebDeploy
#addin Cake.Powershell
#addin "Cake.Docker"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

string version;

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
            var yaml = System.IO.File.ReadAllText("web-prod.yaml");
            yaml = System.Text.RegularExpressions.Regex.Replace(yaml, $@"(image: .*/blog):(\d+\.\d+\.\d+)", $"$1:{version}");
            System.IO.File.WriteAllText("web-prod.yaml", yaml);

            yaml = System.IO.File.ReadAllText("tasks.yaml");
            yaml = System.Text.RegularExpressions.Regex.Replace(yaml, $@"(image: .*/blog-tasks):(\d+\.\d+\.\d+)", $"$1:{version}");
            System.IO.File.WriteAllText("tasks.yaml", yaml);
        });
    });

Task("DockerBuildWeb")
    .IsDependentOn("UpdateDockerFileVersions")
    .Does(() =>
    {
        DockerBuild(new DockerImageBuildSettings { Tag = new[] { $"everstack.azurecr.io/blog:{version}" }, File = "DanClarkeBlog.Web/Dockerfile" }, ".");
    });

Task("DockerBuildTasks")
    .IsDependentOn("UpdateDockerFileVersions")
    .Does(() =>
    {
        DockerBuild(new DockerImageBuildSettings { Tag = new[] { $"everstack.azurecr.io/blog-tasks:{version}" }, File = "DanClarkeBlog.Tasks/Dockerfile" }, ".");
    });

Task("DockerPushWeb")
    .IsDependentOn("CalculateVersionNumber")
    .IsDependentOn("DockerBuildWeb")
    .Does(() =>
    {
        DockerPush($"everstack.azurecr.io/blog:{version}");
    });

Task("DockerPushTasks")
    .IsDependentOn("CalculateVersionNumber")
    .IsDependentOn("DockerBuildTasks")
    .Does(() =>
    {
        DockerPush($"everstack.azurecr.io/blog-tasks:{version}");
    });

Task("K8sApplyConfig")
    .Does(() =>
    {
        StartProcess("kubectl.exe", $"apply -f /Kubernetes/config.yaml");
    });

Task("K8sApplyWeb")
    .IsDependentOn("UpdateK8sVersions")
    .IsDependentOn("DockerPushWeb")
    .IsDependentOn("K8sApplyConfig")
    .Does(() =>
    {
        StartProcess("kubectl.exe", "apply -f Kubernetes/web-prod.yaml");
    });

Task("K8sApplyTasks")
    .IsDependentOn("UpdateK8sVersions")
    .IsDependentOn("DockerPushTasks")
    .IsDependentOn("K8sApplyConfig")
    .Does(() =>
    {
        StartProcess("kubectl.exe", "apply -f Kubernetes/tasks.yaml");
    });

Task("DeployAll")
    .IsDependentOn("DockerPushWeb")
    .IsDependentOn("DockerPushTasks")
    .Does(() => {
    });

Task("ApplyAll")
    .IsDependentOn("K8sApplyWeb")
    .IsDependentOn("K8sApplyTasks")
    .Does(() => {
    });

Task("Default")
    .Does(() => {
        Warning("No default task - explicitly choose DeployAll or ApplyAll. Remember at this time, ApplyAll doesn't handle zero down time swaps!");
    });

RunTarget(target);
