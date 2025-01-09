variable "subscription_id" {
  description = "The subscription ID for Azure resources"
  type        = string
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "darksideleasing-dev-eastus"
  location = "East US"
}

# App Service Plan
resource "azurerm_service_plan" "ui" {
  name                = "darksideleasing-dev-ui-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Windows"
  sku_name            = "B1"

  tags = {
    environment = "dev"
  }
}

# App Service for Blazor Server UI
resource "azurerm_app_service" "ui" {
  name                = "darksideleasing-dev-ui"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  app_service_plan_id = azurerm_service_plan.ui.id

  site_config {
    dotnet_framework_version = "v8.0"  # Updated for .NET 8
    always_on                = true
  }

  tags = {
    environment = "dev"
  }
}

# Serverless SQL Server
resource "azurerm_mssql_server" "db" {
  name                         = "darksideleasing-dev-sql"
  location                     = azurerm_resource_group.main.location
  resource_group_name          = azurerm_resource_group.main.name
  administrator_login          = "sqladmin"
  administrator_login_password = "P@ssw0rd1234!"
  version                      = "12.0"

  tags = {
    environment = "dev"
  }
}

# Serverless SQL Database
resource "azurerm_mssql_database" "db" {
  name      = "darksideleasing-dev-db"
  server_id = azurerm_mssql_server.db.id
  sku_name  = "GP_S_Gen5_2"

  tags = {
    environment = "dev"
  }
}

# Storage Account for Functions
resource "azurerm_storage_account" "functions" {
  name                     = "darksideleasingdevfuncs"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = "dev"
  }
}

# HTTP Triggered Azure Function
resource "azurerm_function_app" "http_api" {
  name                       = "darksideleasing-dev-http-api"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_service_plan.ui.id
  storage_account_name       = azurerm_storage_account.functions.name
  storage_account_access_key = azurerm_storage_account.functions.primary_access_key
  os_type                    = "Windows"

  tags = {
    environment = "dev"
  }
}

# Durable Azure Function
resource "azurerm_function_app" "durable_queue" {
  name                       = "darksideleasing-dev-durable-queue"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_service_plan.ui.id
  storage_account_name       = azurerm_storage_account.functions.name
  storage_account_access_key = azurerm_storage_account.functions.primary_access_key
  os_type                    = "Windows"

  tags = {
    environment = "dev"
  }
}


