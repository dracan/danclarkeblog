Param(
    [string] $ResourceGroup,
    [string] $AppName,
    [string] $DropboxAccessToken,
    [string] $BlogSqlConnectionString,
    [string] $AzureStorageConnectionString,
    [string] $DisqusDomainName
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
$hash['PROJECT'] = 'danclarkeblog.web'

Set-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName -AppSettings $hash