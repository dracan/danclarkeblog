Param(
    [string] $ResourceGroup,
    [string] $AppName,
    [string] $DropboxAccessToken,
    [string] $BlogSqlConnectionString,
    [string] $AzureStorageConnectionString,
    [string] $DisqusDomainName
)

$hash = @{}

$hash['DropboxAccessToken'] = $DropboxAccessToken
$hash['BlogSqlConnectionString'] = $BlogSqlConnectionString
$hash['AzureStorageConnectionString'] = $AzureStorageConnectionString
$hash['DisqusDomainName'] = $DisqusDomainName
$hash['PROJECT'] = 'danclarkeblog.web'

Set-AzureRMWebApp -ResourceGroupName $ResourceGroup -Name $AppName -AppSettings $hash
