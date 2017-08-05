#Requires -Version 3.0
#Requires -Module AzureRM.Resources

Param(
 [Parameter(Mandatory=$True)]
 [string]
 $EnvironmentTag,
  
 [string]
 $ResourceGroupLocation = "North Europe",

 [switch]
 $SkipClusterInCloud
)

# stop the script on first error
$ErrorActionPreference = 'Stop'

#******************************************************************************
# Dependencies
#******************************************************************************

. "DeviceCache.Infrastructure/Common-Functions.ps1"

#******************************************************************************
# Script body
#******************************************************************************

$resourceGroupName = "ca-devcache-$EnvironmentTag-rg"
CreateResourceGroupIfNotPresent -resourceGroupName $ResourceGroupName -resourceGroupLocation $ResourceGroupLocation

$eventHubTemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Infrastructure/EventHub.json"))
$registryTemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Infrastructure/Registry.json"))
$clusterTemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Infrastructure/Cluster.json"))

$keyVaultName = "ca-devcache-$EnvironmentTag"
Create-KeyVault -KeyVaultName $keyVaultName -ResourceGroupName $resourceGroupName -ResourceGroupLocation $ResourceGroupLocation

$eventHubTemplateParameters = New-Object -TypeName Hashtable
$eventHubTemplateParameters["EnvironmentTag"] = $EnvironmentTag

DeployTemplate -ResourceGroupName $resourceGroupName -TemplateFileFullPath $eventHubTemplateFile -TemplateParameters $eventHubTemplateParameters

$registryTemplateParameters = New-Object -TypeName Hashtable
$registryTemplateParameters["EnvironmentTag"] = $EnvironmentTag

DeployTemplate -ResourceGroupName $resourceGroupName -TemplateFileFullPath $registryTemplateFile -TemplateParameters $registryTemplateParameters

if (-not $SkipClusterInCloud) {
    $automationKeyVaultName = "ca-automation-$EnvironmentTag"
    $automationKeyVault = Get-AzureRmKeyVault -VaultName $automationKeyVaultName -ErrorAction SilentlyContinue

    if (-not $automationKeyVault) {
        throw "Automation key vault required for the cluster not found. Make sure you run the Create-CloudClusterPrerequisites script first."
    }

    $clusterManagerId = (Get-AzureKeyVaultSecret -VaultName $automationKeyVaultName -SecretName servicePrincipalId).SecretValueText
    $clusterManagerKey = (Get-AzureKeyVaultSecret -VaultName $automationKeyVaultName -SecretName servicePrincipalPassword).SecretValue
    $sshPublicKey = (Get-AzureKeyVaultSecret -VaultName $automationKeyVaultName -SecretName machineSshPublicKey).SecretValueText

    $clusterTemplateParameters = New-Object -TypeName Hashtable
    $clusterTemplateParameters["EnvironmentTag"] = $EnvironmentTag
    $clusterTemplateParameters["ManagementPrincipalId"] = $clusterManagerId
    $clusterTemplateParameters["ManagementPrincipalKey"] = $clusterManagerKey
    $clusterTemplateParameters["SshPublicKey"] = $sshPublicKey

    DeployTemplate -ResourceGroupName $resourceGroupName -TemplateFileFullPath $clusterTemplateFile -TemplateParameters $clusterTemplateParameters
}