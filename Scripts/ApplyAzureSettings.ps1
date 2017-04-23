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
    [string] $SiteHomeUri
)

$webApp = Get-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName
$appSettingList = $webApp.SiteConfig.AppSettings

$hash = @{}
ForEach ($kvp in $appSettingList) {
    $hash[$kvp.Name] = $kvp.Value
}

$hash['DropboxAccessToken'] = $DropboxAccessToken
$hash['BlogSqlConnectionString'] = $BlogSqlConnectionString
$hash['AzureStorageConnectionString'] = $AzureStorageConnectionString
$hash['DisqusDomainName'] = $DisqusDomainName
$hash['PROJECT'] = $Project
$hash['MaxResizedImageSize'] = $MaxResizedImageSize
$hash['SlackNotificationUri'] = $SlackNotificationUri
$hash['KeepAlivePingUri'] = $KeepAlivePingUri
$hash['SiteHomeUri'] = $SiteHomeUri

Set-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName -AppSettings $hash