<Query Kind="Statements">
  <NuGetReference>Microsoft.Azure.ConfigurationManager</NuGetReference>
  <NuGetReference>Microsoft.Azure.Storage.Queue</NuGetReference>
  <Namespace>Microsoft.Azure.Storage</Namespace>
  <Namespace>Microsoft.Azure.Storage.Queue</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var queue = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("Blog__AzureStorageConnectionString"))
    .CreateCloudQueueClient()
    .GetQueueReference("sync");

await queue.CreateIfNotExistsAsync();

queue.AddMessage(new CloudQueueMessage(
    JsonConvert.SerializeObject(new { IsIncremental = false })
));
