# Azure Infrastructure Guidelines

## Purpose

This document defines comprehensive Azure infrastructure requirements for the Academic Management System, including resource naming conventions, required services with SKU specifications, Bicep infrastructure-as-code templates, and network security configurations to ensure scalable, secure, and maintainable cloud infrastructure.

## Scope

This document covers:

- Azure resource naming conventions and standards
- Required Azure services with appropriate SKU selections
- Bicep template structure and implementation patterns
- Network security and access control configurations
- Environment-specific infrastructure variations
- Cost optimization and resource governance

This document does not cover:

- Application deployment procedures
- Database schema implementation
- Application security configurations
- Performance tuning methodologies

## Prerequisites

- Understanding of Azure services and pricing models
- Familiarity with Infrastructure as Code (IaC) concepts
- Knowledge of Bicep template syntax and best practices
- Understanding of network security principles

## Resource Naming Conventions

### Naming Strategy

All Azure resources follow the pattern: `aca-{environment}-{service}-{instance}`

Where:

- **aca**: Academic Management System abbreviation
- **environment**: Environment identifier (dev, tst, stg, prd)
- **service**: Service/resource type abbreviation
- **instance**: Instance identifier (001, 002, etc.) or specific identifier

### Resource Type Abbreviations

```yaml
Resource Types:
  - Resource Group: rg
  - App Service Plan: asp
  - App Service: app
  - Function App: func
  - SQL Server: sql
  - SQL Database: sqldb
  - Key Vault: kv
  - Storage Account: st (followed by random string for uniqueness)
  - Service Bus Namespace: sb
  - Redis Cache: redis
  - Application Insights: ai
  - Log Analytics Workspace: law
  - Container Registry: cr
  - API Management: apim
  - Virtual Network: vnet
  - Subnet: snet
  - Network Security Group: nsg
  - Application Gateway: agw
  - Load Balancer: lb
```

### Environment-Specific Examples

```bash
# Development Environment
aca-dev-rg-001                    # Resource Group
aca-dev-asp-001                   # App Service Plan
aca-dev-app-api                   # API App Service
aca-dev-app-web                   # Web App Service
aca-dev-sql-001                   # SQL Server
aca-dev-sqldb-students            # Students Database
aca-dev-sqldb-courses             # Courses Database
aca-dev-kv-001                    # Key Vault
aca-dev-st001xyz                  # Storage Account (with random suffix)
aca-dev-sb-001                    # Service Bus
aca-dev-redis-001                 # Redis Cache

# Production Environment
aca-prd-rg-001                    # Resource Group
aca-prd-asp-001                   # Primary App Service Plan
aca-prd-asp-002                   # Secondary App Service Plan (DR)
aca-prd-app-api                   # API App Service
aca-prd-app-web                   # Web App Service
aca-prd-sql-001                   # Primary SQL Server
aca-prd-sql-002                   # Secondary SQL Server (DR)
aca-prd-sqldb-students            # Students Database
aca-prd-kv-001                    # Key Vault
```

## Required Azure Services and SKUs

### Compute Services

```yaml
App Service Plans:
  Development:
    SKU: "B1" # Basic tier for cost optimization
    Instance Count: 1
    Auto-scaling: Disabled

  Testing/Staging:
    SKU: "S1" # Standard tier with staging slots
    Instance Count: 1
    Auto-scaling: Enabled (1-2 instances)

  Production:
    SKU: "P2V3" # Premium tier for performance
    Instance Count: 2
    Auto-scaling: Enabled (2-10 instances)
    Zone Redundancy: Enabled

App Services:
  API Service:
    Runtime: ".NET 8.0"
    Platform: "64-bit"
    Always On: true
    ARR Affinity: false
    Health Check: "/health"

  Web Application:
    Runtime: "Node 18 LTS"
    Platform: "64-bit"
    Always On: true
    ARR Affinity: false
```

### Database Services

```yaml
Azure SQL Database:
  Development:
    Server:
      Location: "East US 2"
      Version: "12.0"
      Administrator Login: "acadmin"

    Databases:
      Students Database:
        SKU: "Basic (5 DTU)"
        Max Size: "2 GB"
        Backup Retention: "7 days"

      Courses Database:
        SKU: "Basic (5 DTU)"
        Max Size: "2 GB"
        Backup Retention: "7 days"

  Production:
    Server:
      Location: "East US 2"
      Version: "12.0"
      Administrator Login: "acadmin"
      Failover Group: Enabled
      Secondary Location: "West US 2"

    Databases:
      Students Database:
        SKU: "S2 (50 DTU)"
        Max Size: "250 GB"
        Backup Retention: "35 days"
        Geo-Replication: Enabled

      Courses Database:
        SKU: "S2 (50 DTU)"
        Max Size: "250 GB"
        Backup Retention: "35 days"
        Geo-Replication: Enabled
```

### Caching and Messaging

```yaml
Azure Cache for Redis:
  Development:
    SKU: "Basic C0 (250 MB)"
    Version: "6.0"

  Production:
    SKU: "Standard C1 (1 GB)"
    Version: "6.0"
    Persistence: Enabled
    Clustering: Disabled

Service Bus:
  Development:
    SKU: "Basic"

  Production:
    SKU: "Standard"
    Auto-scaling: Enabled
    Geo-Disaster Recovery: Enabled
```

### Security and Key Management

```yaml
Key Vault:
  All Environments:
    SKU: "Standard"
    Access Policy Model: "Role-based access control (RBAC)"
    Soft Delete: Enabled (90 days retention)
    Purge Protection: Enabled (Production only)
    Network Access: "Private endpoint and selected networks"

Azure Active Directory:
  B2C Tenant:
    SKU: "Premium P1" (Production)
    Features:
      - Multi-factor Authentication
      - Conditional Access
      - Identity Protection
```

### Monitoring and Analytics

```yaml
Application Insights:
  All Environments:
    Application Type: "web"
    Sampling Percentage: 100% (Dev/Test), 10% (Production)

Log Analytics Workspace:
  All Environments:
    SKU: "PerGB2018"
    Retention: 30 days (Dev), 90 days (Production)
```

### Networking

```yaml
Virtual Network:
  Address Space: "10.0.0.0/16"

  Subnets:
    App Services: "10.0.1.0/24"
    Databases: "10.0.2.0/24"
    Application Gateway: "10.0.3.0/24"
    Private Endpoints: "10.0.4.0/24"

Application Gateway:
  Production Only:
    SKU: "Standard_v2"
    Capacity: "2 instances"
    Auto-scaling: Enabled (2-10 instances)
    Web Application Firewall: Enabled
```

## Bicep Template Structure

### Main Template (main.bicep)

```bicep
targetScope = 'subscription'

@description('Environment name (dev, tst, stg, prd)')
@allowed(['dev', 'tst', 'stg', 'prd'])
param environment string

@description('Location for all resources')
param location string = 'eastus2'

@description('Application name prefix')
param applicationName string = 'aca'

@description('SQL Administrator login')
@secure()
param sqlAdminLogin string

@description('SQL Administrator password')
@secure()
param sqlAdminPassword string

// Variables
var resourceGroupName = '${applicationName}-${environment}-rg-001'
var keyVaultName = '${applicationName}-${environment}-kv-001'

// Resource Group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: {
    Environment: environment
    Application: applicationName
    CreatedBy: 'Bicep'
    CreatedDate: utcNow('yyyy-MM-dd')
  }
}

// Core Infrastructure Module
module coreInfrastructure 'modules/core-infrastructure.bicep' = {
  scope: resourceGroup
  name: 'core-infrastructure'
  params: {
    environment: environment
    location: location
    applicationName: applicationName
    keyVaultName: keyVaultName
  }
}

// Database Module
module database 'modules/database.bicep' = {
  scope: resourceGroup
  name: 'database'
  params: {
    environment: environment
    location: location
    applicationName: applicationName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    keyVaultName: keyVaultName
  }
  dependsOn: [
    coreInfrastructure
  ]
}

// App Services Module
module appServices 'modules/app-services.bicep' = {
  scope: resourceGroup
  name: 'app-services'
  params: {
    environment: environment
    location: location
    applicationName: applicationName
    keyVaultName: keyVaultName
  }
  dependsOn: [
    database
  ]
}

// Monitoring Module
module monitoring 'modules/monitoring.bicep' = {
  scope: resourceGroup
  name: 'monitoring'
  params: {
    environment: environment
    location: location
    applicationName: applicationName
  }
}

// Outputs
output resourceGroupName string = resourceGroup.name
output keyVaultName string = coreInfrastructure.outputs.keyVaultName
output sqlServerName string = database.outputs.sqlServerName
output appServicePlanName string = appServices.outputs.appServicePlanName
```

### Core Infrastructure Module (modules/core-infrastructure.bicep)

```bicep
@description('Environment name')
param environment string

@description('Location for resources')
param location string

@description('Application name prefix')
param applicationName string

@description('Key Vault name')
param keyVaultName string

// Variables
var storageAccountName = toLower('${applicationName}${environment}st${uniqueString(resourceGroup().id)}')
var serviceBusName = '${applicationName}-${environment}-sb-001'
var redisCacheName = '${applicationName}-${environment}-redis-001'

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: environment == 'prd' ? 'Standard_ZRS' : 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    defaultToOAuthAuthentication: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }

  resource blobService 'blobServices@2023-01-01' = {
    name: 'default'
    properties: {
      deleteRetentionPolicy: {
        enabled: true
        days: environment == 'prd' ? 30 : 7
      }
    }
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    enabledForDeployment: false
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: environment == 'prd' ? true : false
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// Service Bus Namespace
resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: environment == 'prd' ? 'Standard' : 'Basic'
    tier: environment == 'prd' ? 'Standard' : 'Basic'
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: false
  }
}

// Service Bus Queues
resource enrollmentQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: 'enrollment-events'
  properties: {
    defaultMessageTimeToLive: 'P14D'
    maxDeliveryCount: 10
    enablePartitioning: false
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    requiresDuplicateDetection: true
  }
}

resource gradeQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: 'grade-events'
  properties: {
    defaultMessageTimeToLive: 'P14D'
    maxDeliveryCount: 10
    enablePartitioning: false
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    requiresDuplicateDetection: true
  }
}

// Redis Cache
resource redisCache 'Microsoft.Cache/redis@2023-08-01' = {
  name: redisCacheName
  location: location
  properties: {
    sku: {
      name: environment == 'prd' ? 'Standard' : 'Basic'
      family: 'C'
      capacity: environment == 'prd' ? 1 : 0
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    redisConfiguration: {
      'maxmemory-reserved': environment == 'prd' ? '125' : '30'
      'maxfragmentationmemory-reserved': environment == 'prd' ? '125' : '30'
    }
  }
}

// Outputs
output storageAccountName string = storageAccount.name
output keyVaultName string = keyVault.name
output serviceBusNamespaceName string = serviceBusNamespace.name
output redisCacheName string = redisCache.name
```

### Database Module (modules/database.bicep)

```bicep
@description('Environment name')
param environment string

@description('Location for resources')
param location string

@description('Application name prefix')
param applicationName string

@description('SQL Admin login')
@secure()
param sqlAdminLogin string

@description('SQL Admin password')
@secure()
param sqlAdminPassword string

@description('Key Vault name for storing connection strings')
param keyVaultName string

// Variables
var sqlServerName = '${applicationName}-${environment}-sql-001'
var studentsDbName = '${applicationName}-${environment}-sqldb-students'
var coursesDbName = '${applicationName}-${environment}-sqldb-courses'

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled' // Will be restricted by firewall rules
  }
}

// SQL Server Firewall Rules
resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Students Database
resource studentsDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: studentsDbName
  location: location
  sku: {
    name: environment == 'prd' ? 'S2' : 'Basic'
    tier: environment == 'prd' ? 'Standard' : 'Basic'
    capacity: environment == 'prd' ? 50 : 5
  }
  properties: {
    maxSizeBytes: environment == 'prd' ? 268435456000 : 2147483648 // 250GB : 2GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: environment == 'prd' ? true : false
  }
}

// Courses Database
resource coursesDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: coursesDbName
  location: location
  sku: {
    name: environment == 'prd' ? 'S2' : 'Basic'
    tier: environment == 'prd' ? 'Standard' : 'Basic'
    capacity: environment == 'prd' ? 50 : 5
  }
  properties: {
    maxSizeBytes: environment == 'prd' ? 268435456000 : 2147483648 // 250GB : 2GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: environment == 'prd' ? true : false
  }
}

// Store connection strings in Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource studentsConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'StudentsDatabaseConnectionString'
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${studentsDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

resource coursesConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'CoursesDatabaseConnectionString'
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${coursesDatabase.name};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

// Outputs
output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output studentsDatabaseName string = studentsDatabase.name
output coursesDatabaseName string = coursesDatabase.name
```

### App Services Module (modules/app-services.bicep)

```bicep
@description('Environment name')
param environment string

@description('Location for resources')
param location string

@description('Application name prefix')
param applicationName string

@description('Key Vault name')
param keyVaultName string

// Variables
var appServicePlanName = '${applicationName}-${environment}-asp-001'
var apiAppName = '${applicationName}-${environment}-app-api'
var webAppName = '${applicationName}-${environment}-app-web'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: environment == 'prd' ? 'P2V3' : (environment == 'stg' ? 'S1' : 'B1')
    capacity: environment == 'prd' ? 2 : 1
  }
  properties: {
    zoneRedundant: environment == 'prd' ? true : false
  }
}

// API App Service
resource apiApp 'Microsoft.Web/sites@2023-01-01' = {
  name: apiAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prd' ? 'Production' : 'Development'
        }
        {
          name: 'KeyVaultName'
          value: keyVaultName
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Web App Service
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      nodeVersion: '18-lts'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'NODE_ENV'
          value: environment == 'prd' ? 'production' : 'development'
        }
        {
          name: 'API_BASE_URL'
          value: 'https://${apiApp.properties.defaultHostName}/api'
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Application Insights
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${applicationName}-${environment}-ai-001'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    SamplingPercentage: environment == 'prd' ? 10 : 100
  }
}

// Grant Key Vault access to API App
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource apiAppKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, apiApp.id, 'Key Vault Secrets User')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: apiApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// Auto-scaling settings for production
resource autoScaleSettings 'Microsoft.Insights/autoscalesettings@2022-10-01' = if (environment == 'prd') {
  name: '${appServicePlanName}-autoscale'
  location: location
  properties: {
    profiles: [
      {
        name: 'Default'
        capacity: {
          minimum: '2'
          maximum: '10'
          default: '2'
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 70
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
          }
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT10M'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: 30
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
        ]
      }
    ]
    enabled: true
    targetResourceUri: appServicePlan.id
  }
}

// Outputs
output appServicePlanName string = appServicePlan.name
output apiAppName string = apiApp.name
output webAppName string = webApp.name
output applicationInsightsConnectionString string = applicationInsights.properties.ConnectionString
```

## Network Security Configuration

### Network Security Groups

```bicep
// Network Security Group for App Services
resource appServiceNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: '${applicationName}-${environment}-nsg-app'
  location: location
  properties: {
    securityRules: [
      {
        name: 'AllowHttpsInbound'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: 'Internet'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1000
          direction: 'Inbound'
        }
      }
      {
        name: 'AllowHttpInbound'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '80'
          sourceAddressPrefix: 'Internet'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1001
          direction: 'Inbound'
        }
      }
      {
        name: 'DenyAllInbound'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 4096
          direction: 'Inbound'
        }
      }
    ]
  }
}

// Network Security Group for Database Subnet
resource databaseNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: '${applicationName}-${environment}-nsg-db'
  location: location
  properties: {
    securityRules: [
      {
        name: 'AllowAppServiceToDatabase'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '1433'
          sourceAddressPrefix: '10.0.1.0/24' // App Service subnet
          destinationAddressPrefix: '10.0.2.0/24' // Database subnet
          access: 'Allow'
          priority: 1000
          direction: 'Inbound'
        }
      }
      {
        name: 'DenyAllInbound'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 4096
          direction: 'Inbound'
        }
      }
    ]
  }
}
```

### Virtual Network Configuration

```bicep
// Virtual Network
resource vnet 'Microsoft.Network/virtualNetworks@2023-05-01' = {
  name: '${applicationName}-${environment}-vnet-001'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'app-services'
        properties: {
          addressPrefix: '10.0.1.0/24'
          networkSecurityGroup: {
            id: appServiceNsg.id
          }
          delegations: [
            {
              name: 'Microsoft.Web/serverFarms'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
      {
        name: 'database'
        properties: {
          addressPrefix: '10.0.2.0/24'
          networkSecurityGroup: {
            id: databaseNsg.id
          }
        }
      }
      {
        name: 'private-endpoints'
        properties: {
          addressPrefix: '10.0.4.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
    ]
  }
}
```

## Deployment Scripts

### PowerShell Deployment Script

```powershell
#Requires -Version 7.0
#Requires -Modules Az

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('dev', 'tst', 'stg', 'prd')]
    [string]$Environment,

    [Parameter(Mandatory = $true)]
    [string]$SubscriptionId,

    [Parameter(Mandatory = $false)]
    [string]$Location = 'eastus2',

    [Parameter(Mandatory = $false)]
    [string]$ResourceGroupName = "aca-$Environment-rg-001"
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Connect to Azure
Write-Host "Connecting to Azure subscription: $SubscriptionId" -ForegroundColor Green
Connect-AzAccount
Set-AzContext -SubscriptionId $SubscriptionId

# Deploy infrastructure
Write-Host "Deploying Academic Management System infrastructure to $Environment environment" -ForegroundColor Green

try {
    # Create deployment parameters
    $deploymentParams = @{
        environment = $Environment
        location = $Location
        sqlAdminLogin = Read-Host -Prompt "Enter SQL Administrator login"
        sqlAdminPassword = Read-Host -Prompt "Enter SQL Administrator password" -AsSecureString
    }

    # Deploy main template
    $deployment = New-AzSubscriptionDeployment `
        -Name "aca-$Environment-$(Get-Date -Format 'yyyyMMdd-HHmmss')" `
        -Location $Location `
        -TemplateFile "infrastructure/main.bicep" `
        -TemplateParameterObject $deploymentParams `
        -Verbose

    if ($deployment.ProvisioningState -eq 'Succeeded') {
        Write-Host "Infrastructure deployment completed successfully!" -ForegroundColor Green

        # Output key information
        Write-Host "Resource Group: $($deployment.Outputs.resourceGroupName.Value)" -ForegroundColor Yellow
        Write-Host "Key Vault: $($deployment.Outputs.keyVaultName.Value)" -ForegroundColor Yellow
        Write-Host "SQL Server: $($deployment.Outputs.sqlServerName.Value)" -ForegroundColor Yellow
    }
    else {
        Write-Error "Infrastructure deployment failed with state: $($deployment.ProvisioningState)"
    }
}
catch {
    Write-Error "Error during deployment: $_"
}
```

## Environment-Specific Configurations

### Development Environment

```yaml
Purpose: Development and unit testing
Resources:
  - Minimal SKUs for cost optimization
  - Single-instance deployments
  - Basic backup retention
  - Simplified monitoring

Security:
  - Relaxed network restrictions for development access
  - Basic authentication requirements
  - Standard encryption (no premium features)

Cost Optimization:
  - Basic/Standard SKUs
  - No geo-replication
  - Shorter retention periods
  - Manual scaling only
```

### Production Environment

```yaml
Purpose: Live production workloads
Resources:
  - Premium SKUs for performance and reliability
  - Multi-instance deployments with auto-scaling
  - Extended backup retention and geo-replication
  - Comprehensive monitoring and alerting

Security:
  - Strict network access controls
  - Multi-factor authentication required
  - Premium security features enabled
  - Private endpoints for sensitive resources

High Availability:
  - Zone-redundant deployments
  - Geo-replication for databases
  - Auto-failover capabilities
  - Disaster recovery procedures
```

## Cost Optimization Strategies

### Resource Governance

```yaml
Azure Policy Assignments:
  - Enforce resource naming conventions
  - Restrict allowed SKUs by environment
  - Require specific tags on all resources
  - Prevent creation of expensive resources in non-prod

Cost Management:
  - Budget alerts at 50%, 80%, and 100% thresholds
  - Resource group spending limits
  - Automated shutdown for non-production environments
  - Regular cost reviews and optimization recommendations
```

### Automation and Monitoring

```yaml
Automated Actions:
  - Scale down non-production resources after hours
  - Delete unused resources automatically
  - Right-size recommendations implementation
  - Reserved instance purchase recommendations

Monitoring:
  - Cost anomaly detection
  - Resource utilization tracking
  - Performance vs. cost analysis
  - Monthly cost reporting by resource group
```

## Related Documentation References

- [Configuration Management](./configuration-management.instructions.md)
- [Security and Compliance](./security-compliance.instructions.md)
- [Deployment Operations](./deployment-operations.instructions.md)
- [Monitoring and Observability](./monitoring-observability.instructions.md)

## Validation Checklist

Before considering the Azure infrastructure implementation complete, verify:

- [ ] Resource naming conventions follow established standards consistently
- [ ] Required Azure services are defined with appropriate SKUs for each environment
- [ ] Bicep templates are modular, reusable, and follow best practices
- [ ] Network security configurations implement least-privilege access
- [ ] Cost optimization strategies are implemented and monitored
- [ ] Infrastructure supports disaster recovery and business continuity requirements
- [ ] Deployment automation scripts handle error scenarios gracefully
- [ ] Environment-specific configurations address security and compliance needs
- [ ] Resource governance policies prevent unauthorized resource creation
- [ ] Monitoring and alerting cover infrastructure health and cost management
- [ ] All sensitive information is stored securely in Key Vault
- [ ] Infrastructure templates support both development and production workloads
- [ ] Documentation includes troubleshooting and operational procedures
