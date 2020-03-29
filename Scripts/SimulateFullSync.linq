<Query Kind="Program">
  <Reference>C:\Code\danclarkeblog\DanClarkeBlog.Core\bin\Debug\netstandard2.0\DanClarkeBlog.Core.dll</Reference>
  <NuGetReference>Autofac</NuGetReference>
  <NuGetReference>Markdig</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.SqlServer</NuGetReference>
  <NuGetReference>Polly</NuGetReference>
  <NuGetReference>Serilog.Sinks.Debug</NuGetReference>
  <NuGetReference>WindowsAzure.Storage</NuGetReference>
  <Namespace>Autofac</Namespace>
  <Namespace>DanClarkeBlog.Core</Namespace>
  <Namespace>DanClarkeBlog.Core.Helpers</Namespace>
  <Namespace>DanClarkeBlog.Core.Repositories</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Serilog</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    var container = Setup();

    const bool isIncremental = false;

    var syncHelper = container.Resolve<SyncHelper>();
    var sourceRepository = container.Resolve<IBlogPostSourceRepository>();
    var targetRepository = container.Resolve<IBlogPostTargetRepository>();
    
//    (targetRepository as BlogPostSqlServerRepository)?.CreateDatabase();
    
    await syncHelper.SynchronizeBlogPostsAsync(
        (IBlogPostRepository)sourceRepository,
        (IBlogPostRepository)targetRepository,
        isIncremental,
        null,
        CancellationToken.None);

    Log.Debug("Finished dropbox sync");
}

IContainer Setup()
{
    var container = new ContainerBuilder();

    var settings = new Settings
    {
        DropboxAccessToken = Environment.GetEnvironmentVariable("Blog__DropboxAccessToken"),
        BlogSqlConnectionString = Environment.GetEnvironmentVariable("Blog__BlogSqlConnectionString"),
        AzureStorageConnectionString = Environment.GetEnvironmentVariable("Blog__AzureStorageConnectionString"),
        MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("Blog__MaxResizedImageSize") ?? "0"),
        KeepAlivePingUri = Environment.GetEnvironmentVariable("Blog__KeepAlivePingUri"),
        SlackNotificationUri = Environment.GetEnvironmentVariable("Blog__SlackNotificationUri"),
        PostPreviewLength = int.Parse(Environment.GetEnvironmentVariable("Blog__PostPreviewLength") ?? "200"),
        BaseImageUri = Environment.GetEnvironmentVariable("Blog__BaseImageUri"),
    };

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Debug()
        .MinimumLevel.Debug()
        .CreateLogger();

    container.Register(x => settings);
    container.RegisterType<BlogPostSqlServerRepository>().As<IBlogPostTargetRepository>().SingleInstance();
    container.RegisterType<BlogPostDropboxRepository>().As<IBlogPostSourceRepository>().SingleInstance();
    container.RegisterType<BlogPostSummaryHelper>().SingleInstance();
    container.RegisterType<BlogPostMarkdownRenderer>().As<IBlogPostRenderer>().SingleInstance();
    container.RegisterType<AzureImageRepository>().As<IImageRepository>().SingleInstance();
    container.RegisterType<ImageResizer>().As<IImageResizer>().SingleInstance();
    container.RegisterType<SyncHelper>().SingleInstance();
    container.RegisterType<DropboxHelper>().As<IDropboxHelper>().SingleInstance();
    container.RegisterType<HttpClientHelper>().As<IHttpClientHelper>().SingleInstance();
    container.RegisterType<SlackNotificationTarget>().As<INotificationTarget>().SingleInstance();
    container.RegisterType<FeedGenerator>().As<IFeedGenerator>().SingleInstance();
    container.RegisterType<HashVerify>().As<IHashVerify>().SingleInstance();
    container.RegisterType<AzureBlobLockRepository>().As<ILockRepository>().SingleInstance();
    container.RegisterType<AzureStorageQueue>().As<IMessageQueue>().SingleInstance();
    
    return container.Build();
}

public class SyncMessage
{
    public bool IsIncremental { get; set; }
}