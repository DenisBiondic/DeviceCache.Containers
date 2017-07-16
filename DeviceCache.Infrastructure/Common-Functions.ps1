#Requires -Version 3.0
#Requires -Module AzureRM.Resources

Function CreateResourceGroupIfNotPresent([string]$ResourceGroupName, [string]$ResourceGroupLocation) {
	$resourceGroup = Get-AzureRmResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
	if(!$resourceGroup) {
		Write-Host "Creating resource group '$ResourceGroupName' in location '$ResourceGroupLocation'";
		New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation
	} else {
		Write-Host "Using existing resource group '$ResourceGroupName'";
	}
}

Function DeployTemplate([string]$ResourceGroupName, [string]$TemplateFileFullPath, [Hashtable]$TemplateParameters, [switch]$ValidateOnly) {
	if ($ValidateOnly) {
		$ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
										-TemplateFile $TemplateFileFullPath `
										@TemplateParameters)

	   if ($ErrorMessages) {
			Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
            throw 'Template validation failed'
		}
		else {
			Write-Output '', 'Template is valid.'
		}
	}
	else {
		$TemplateFileName = Split-Path $TemplateFileFullPath -leaf
		$DeploymentName = $TemplateFileName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm') 

		New-AzureRmResourceGroupDeployment -Name $DeploymentName `
										   -ResourceGroupName $ResourceGroupName `
										   -TemplateFile $TemplateFileFullPath `
										   @TemplateParameters `
										   -Force -Verbose `
										   -ErrorVariable ErrorMessages
		if ($ErrorMessages) {
			Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
            throw 'Template deployment failed'
		}
	}
}

Function Create-KeyVault([string]$KeyVaultName, [string]$ResourceGroupName, [string]$ResourceGroupLocation) {
    # due to different problems with ARM templates and key vaults, an actually easier way of creating them is using powershell directly
    # (less bugs, direct assignment of creating user as admin etc.)
    $keyVault = Get-AzureRmKeyVault -VaultName $KeyVaultName -ErrorAction SilentlyContinue

    if (-not $keyVault) {
        New-AzureRmKeyVault -VaultName $KeyVaultName  `
            -ResourceGroupName $ResourceGroupName  `
            -Location $ResourceGroupLocation `
            -EnabledForDeployment `
            -EnabledForTemplateDeployment
    } else {
        Write-Host "Key vault already exists"
    }
}
