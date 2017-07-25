## Required Software:

**Editor**: Visual Studio 2017 / Visual Studio Code

**Azure powershell**: https://github.com/Azure/azure-powershell/releases

**Kubectl**: https://kubernetes.io/docs/tasks/tools/install-kubectl/

**Helm**: https://github.com/kubernetes/helm/blob/master/docs/install.md

## Recommended Software:

**Minikube** (for local development): https://github.com/kubernetes/minikube

**Azure CLI 2.0**

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

Warning: make sure to delete the resources since they incure Azure costs! :)