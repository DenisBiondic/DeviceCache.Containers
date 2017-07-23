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

helm install ./devicecache --set global.imageRepository=$registryUrl