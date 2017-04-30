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
    [string] $ProfilePicUri
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

Set-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName -AppSettings $hash