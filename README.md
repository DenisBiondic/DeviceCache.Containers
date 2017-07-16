# Device Cache - Containers

## Required Software:

**Editor**: Visual Studio Code (but can be opened as folder in Visual Studio as well)

**Azure powershell**: https://github.com/Azure/azure-powershell/releases<br/>(optional) **Azure CLI 2.0**

## Infrastructure Setup

For setting up the neccessary Azure Infrastructure (Infrastructure-as-Code) for the code to run, you can use the Create-Infrastructure.ps1 script. However, this script has a dependency on a Key Vault which should contain deployment-time secrets (service principal, SSH Key, passwords and such).

To create such Key Vault with all required secrets, run the Create-Prerequisites.ps1 script.
First, execute this like to securely enter your credentials for the service principal:

```powershell
$servicePrincipalCredentials = Get-Credential
```

Afterwards, you can execute the Create-Prerequisites.ps1 script itself: 

``` powershell
.\Create-Prerequisites.ps1 -EnvironmentTag "white" -MachineSshPublicKey "ssh-rsa AAAA...6SkIQ0opBt" -ServicePrincipalCredentials $servicePrincipalCredentials
```