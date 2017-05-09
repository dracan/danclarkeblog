Param(
    [string] $ResourceGroup,
    [string] $AppName,
    [string] $Project,
    [string] $DropboxAccessToken,
    [string] $BlogSqlConnectionString,
    [string] $AzureStorageConnectionString,
    [string] $DisqusDomainName,
    [string] $MaxResizedImageSize,
    [string] $SlackNotificationUri,
    [string] $KeepAlivePingUri,
    [string] $SiteHomeUri,
    [string] $DropboxAppSecret,
    [string] $ProfilePicUri,
    [string] $ApplicationInsightsInstrumentationKey,
    [string] $ApplicationInsightsDeveloperMode,
    [string] $PostPreviewLength,
    [string] $BaseImageUri
)

$webApp = Get-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName
$appSettingList = $webApp.SiteConfig.AppSettings

$hash = @{}
ForEach ($kvp in $appSettingList) {
    $hash[$kvp.Name] = $kvp.Value
}

$hash['PROJECT'] = $Project
$hash['Blog:DropboxAccessToken'] = $DropboxAccessToken
$hash['Blog:BlogSqlConnectionString'] = $BlogSqlConnectionString
$hash['Blog:AzureStorageConnectionString'] = $AzureStorageConnectionString
$hash['Blog:DisqusDomainName'] = $DisqusDomainName
$hash['Blog:MaxResizedImageSize'] = $MaxResizedImageSize
$hash['Blog:SlackNotificationUri'] = $SlackNotificationUri
$hash['Blog:KeepAlivePingUri'] = $KeepAlivePingUri
$hash['Blog:SiteHomeUri'] = $SiteHomeUri
$hash['Blog:DropboxAppSecret'] = $DropboxAppSecret
$hash['Blog:ProfilePicUri'] = $ProfilePicUri
$hash['Blog:ApplicationInsightsInstrumentationKey'] = $ApplicationInsightsInstrumentationKey
$hash['Blog:ApplicationInsightsDeveloperMode'] = $ApplicationInsightsDeveloperMode
$hash['Blog:PostPreviewLength'] = $PostPreviewLength
$hash['Blog:BaseImageUri'] = $BaseImageUri

Set-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName -AppSettings $hash