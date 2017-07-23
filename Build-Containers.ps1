#Requires -Version 3.0

Param(
 [Parameter(Mandatory=$True)]
 [string]
 $EnvironmentTag,

 [string]
 $VersionTag = "latest"
)

# stop the script on first error
$ErrorActionPreference = 'Stop'

#******************************************************************************
# Script body
#******************************************************************************

Write-Host "Logging in into the Azure Container Registry..."

$keyVaultName = "ca-devcache-$EnvironmentTag"

$registryUsername = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName registryAdminUsername).SecretValueText
$registryPassword = (Get-AzureKeyVaultSecret -VaultName $keyVaultName -SecretName registryAdminPassword).SecretValueText

$registryUrl = ("cadevcache" + $EnvironmentTag + "registry.azurecr.io")

docker login $registryUrl -u $registryUsername -p $registryPassword

$frontEndContainerTag = "devcache-frontend:$VersionTag"
Write-Host "`r`n[BUILD CONTAINER] Building $frontEndContainerTag container" -foreground "green"
 
$frontendContext = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Frontend"))
$frontendDockerfile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Frontend/Dockerfile"))

docker build -t $frontEndContainerTag -f $frontendDockerfile $frontendContext
docker tag $frontEndContainerTag $registryUrl/$frontEndContainerTag

Write-Host "[PUSH TO REPOSITORY] Pushing $frontEndContainerTag container to $registryUrl" -foreground "green"

docker push $registryUrl/$frontEndContainerTag

Write-Host "[FINISHED] Building $frontEndContainerTag container" -foreground "green"