#Requires -Version 3.0
#Requires -Module AzureRM.Resources

Param(
 [Parameter(Mandatory=$True)]
 [string]
 $EnvironmentTag,

 [Parameter(Mandatory=$True)]
 [string]
 $MachineSshPublicKey,

 [Parameter(Mandatory=$True)]
 [pscredential]
 $ServicePrincipalCredentials,

 [string]
 $ResourceGroupLocation = "North Europe"
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
CreateResourceGroupIfNotPresent -ResourceGroupName $ResourceGroupName -ResourceGroupLocation $ResourceGroupLocation

Write-Host "Setting up key vault with secrets..." 
Write-Host "Creating the vault..."

$automationKeyVaultName = "ca-automation-$EnvironmentTag"
Create-KeyVault -KeyVaultName $automationKeyVaultName -ResourceGroupName $resourceGroupName -ResourceGroupLocation $ResourceGroupLocation

Write-Host "Populating the secrets..."
$automationTemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "DeviceCache.Infrastructure/AutomationSecrets.json"))
Write-Host "Template located at $automationTemplateFile"

$automationTemplateParameters = New-Object -TypeName Hashtable
$automationTemplateParameters["KeyVaultName"] = $automationKeyVaultName
$automationTemplateParameters["MachineSshPublicKey"] = $MachineSshPublicKey
$automationTemplateParameters["ServicePrincipalId"] = $ServicePrincipalCredentials.UserName
$automationTemplateParameters["ServicePrincipalPassword"] = $ServicePrincipalCredentials.Password

DeployTemplate -ResourceGroupName $resourceGroupName -TemplateFileFullPath $automationTemplateFile -TemplateParameters $automationTemplateParameters