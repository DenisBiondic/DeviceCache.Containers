# Setup

## Required Software:

**Editor**: Visual Studio 2017 / Visual Studio Code

**Azure powershell**: https://github.com/Azure/azure-powershell/releases

**Docker** (e.g. for windows: https://docs.docker.com/docker-for-windows/)

**Kubectl**: https://kubernetes.io/docs/tasks/tools/install-kubectl/

**Helm**: https://github.com/kubernetes/helm/blob/master/docs/install.md

## Recommended Software:

**Minikube** (for local development): https://github.com/kubernetes/minikube

**Azure CLI 2.0** (can be run as [docker image](https://hub.docker.com/r/microsoft/azure-cli/))

## Concept of environments

All the PS scripts have the mandatory `-EnvironmentTag` flag which needs to be set. This flag is one of the best practices when writing Infrastructure-as-Code scripts to be able to reuse same scripts between multiple environments. It also helps when multiple people are using the same scripts for their "local" environments so that they get no conflicts in the cloud (each has its own environment).

Important here to know is:
- choose a 2-4 letter [a-Z] tag which you want
- provide the same tag to all the scripts you will be executing

## Prerequisites

This project can be run in two ways:
- with a local cluster (e.g. minikube) while the PaaS components are in cloud (e.g. Event Hub)
- completely in cloud (cluster with Azure Container Services)

Depending whether you want local or cloud deployment, there are some small differences in setup.

If you want to deploy the sample to Azure Container Services (cloud), you need to run the `Create-CloudClusterPrerequisites.ps1` script which should setup a KeyVault with deployment-time secrets (service principal, SSH Key, passwords and such) neccessary for Azure Container Services cluster to created.

More info here: [Runnning Create-CloudClusterPrerequisites script](cloud-prerequisites.md)

## Infrastructure Setup

For setting up the neccessary Azure Infrastructure (Infrastructure-as-Code) for the code to run, you can use the `Create-Infrastructure.ps1` script. 

First, make sure you log in to your Azure Subscription with

```powershell
Login-AzureRmAccount
```

and, if necessary, switch to the correct subscription using

```powershell
# to find out the subscirption id, run Get-AzureRmSubscription
Select-AzureRmSubscription
```

You can execute the `Create-Infrastructure.ps1` script now. In case if you will be working with a local cluster (e.g. minikube), run the following:

```powershell
.\Create-Infrastructure.ps1 -EnvironmentTag <<set_tag_here>> -SkipClusterInCloud
```

If you are going to be using a cloud cluster (ACS), omit the -SkipClusterInCloud flag.

```powershell
# make sure you execute the Create-CloudClusterPrerequisites.ps1 script first!
# more info above in this document, or read cloud-prerequisites.md
.\Create-Infrastructure.ps1 -EnvironmentTag <<your_tag_here>>
```

Script should finish without any errors.

## Initialize the cluster

Before you start deploying the microservices, your cluster needs to be "initialized" first. What this actually means is that we need to write in a secret for private docker registry connections and that we need to initialize [Helm](https://helm.sh/) for doing the actual deployments.

#### Local cluster

If you already installed minikube and are planning to deploy locally, you can go ahead and run the script.

```powershell
.\Initialize-Cluster.ps1 -EnvironmentTag <<your_tag_here>>
```

#### Cloud cluster

If you are planning to use the cloud cluster (ACS), you need to configure your kubectl tool first. Easiest way to do this is through Azure CLI:

```bash
az acs kubernetes get-credentials --resource-group=... --name=...
```

Afterwards, simple run the script:

```powershell
.\Initialize-Cluster.ps1 -EnvironmentTag <<your_tag_here>>
```

#### Kubectl & contexts

Important thing to realise here is that many of the tools like Helm and scripts you will be using in this project are supporting [Kubectl contexts](https://kubernetes.io/docs/tasks/access-application-cluster/authenticate-across-clusters-kubeconfig/) directly. One such context for your local cluster is setup when starting minikube, and the `az acs kubernetes get-credentials ... ` command above also sets up a context for your remote ACS cluster.

## Building the containers

**Short answer:**

```powershell
# builds all the containers and pushes them to a remove environment specific docker registry (ACR)
.\Build-Containers.ps1 -EnvironmentTag <<your_tag_here>>
```

**Long answer:**

One of the things that was setup in previous step, was a private Docker Registry in form of Azure Container Registry. For both scenarios, local and cloud, we will be using this registry to roll out the containers onto the Kubernetes cluster. 

Each of the microservices in this project has a Dockerfile which can be used out of the box and could do the classic docker build / docker tag / docker login / docker push if you wanted (or you can use something like [Draft](https://github.com/Azure/draft))

## Deploying the application

Simply use the Deploy-Application.ps1 script:

```powershell
# will use your current kubectl context for deployment target
.\Deploy-Application.ps1 -EnvironmentTag <<your_tag_here>>
```

Once installed, you will be able to see the pods with `kubectl get pods`. 

`helm list` will also show you your new release.

## Deleting the application

Simply run `helm delete <<release_name>>` with the release name you got from `helm list`.
