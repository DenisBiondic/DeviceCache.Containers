#Requires -Version 3.0

Param(
	[Parameter(Mandatory=$True)]
	[string]
	$EnvironmentTag
)

# stop the script on first error
$ErrorActionPreference = 'Stop'

#******************************************************************************
# Script body
#******************************************************************************

Write-Host "Deploying the application..."

$registryUrl = ("cadevcache" + $EnvironmentTag + "registry.azurecr.io")

$keyVaultName = "ca-devcache-$EnvironmentTag"

$recieveConnectionString = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName eventHubReceiveConnectionString).SecretValueText
$sendConnectionString = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName eventHubSendConnectionString).SecretValueText
$storageAccountKey = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName cadevcachegreenstorage).SecretValueText
$storageAccountName = ("cadevcache" + $EnvironmentTag + "storage")
$eventHubReaderPath = "ca-devcache-$EnvironmentTag-hub"

helm install ./devicecache --set global.imageRepository=$registryUrl `
	--set eventHubReaderConnectionString=$recieveConnectionString `
	--set eventHubSenderConnectionString=$sendConnectionString `
	--set storageAccountName=$storageAccountName `
	--set storageAccountKey=$storageAccountKey `
	--set eventHubReaderPath=$eventHubReaderPath