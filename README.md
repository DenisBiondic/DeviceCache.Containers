# Device Cache - Containers

Simple .NET microservice application based on Microsoft Azure, Kubernetes (Azure Container Service), Helm & Powershell

Follows the [12 Factor App](https://12factor.net/) approach and is built using cloud-native principle of [CaaS](http://blog.kubernetes.io/2017/02/caas-the-foundation-for-next-gen-paas.html) for stateless services (Kubernetes) and **PaaS** for stateful workloads (in this case Azure EventHub)

Implemented scenario showcases handling events from Event Hubs and storing the last information recieved (common IoT cloud use case) into a cache (Redis) available for querying (high performance / throughput scenario) - kind of a CQRS thing for IoT :)

## Required Software:

**Editor**: Visual Studio 2017 / Visual Studio Code

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

Warning: make sure to delete the resources since they incure Azure costs! :)