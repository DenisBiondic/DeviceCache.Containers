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

Write-Host "Initializing cluster... Using kubectl context: " 
kubectl config current-context

Write-Host "Initializing helm..."
helm init

Write-Host "Initializing registry secret..."

$imagePullSecreteName = "devicecache-registry"
$registryUrl = ("cadevcache" + $EnvironmentTag + "registry.azurecr.io")

$keyVaultName = "ca-devcache-$EnvironmentTag"

$registryUsername = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName registryAdminUsername).SecretValueText
$registryPassword = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName registryAdminPassword).SecretValueText

kubectl create secret docker-registry $imagePullSecreteName --docker-server=$registryUrl --docker-username=$registryUsername --docker-password=$registryPassword --docker-email=reallynotimportant@contoso.com