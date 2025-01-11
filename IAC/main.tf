variable "subscription_id" {
  description = "The subscription ID for Azure resources"
  type        = string
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "darksideleasing-demo"
  location = "East US 2"  # Use a global region with high availability
}

# SQL Server (Lowest-Cost Configurations)
resource "azurerm_mssql_server" "db" {
  name                         = "darksideleasing-demo-sql"
  location                     = azurerm_resource_group.main.location
  resource_group_name          = azurerm_resource_group.main.name
  administrator_login          = "sqladmin"
  administrator_login_password = "P@ssw0rd1234!"
  version                      = "12.0"  # SQL Server 2014  

  tags = {
    environment = "demo"
  }
}

# SQL Database (Smallest Configurations)
resource "azurerm_mssql_database" "db" {
  name                = "darksideleasing-demo-db"
  server_id           = azurerm_mssql_server.db.id
  sku_name            = "Basic"  # Basic tier for minimal cost
  max_size_gb         = 2        # Smallest size allowed

  tags = {
    environment = "demo"
  }
}

# Storage Account for Functions
resource "azurerm_storage_account" "functions" {
  name                     = "darksideleasingdemo"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = "demo"
  }
}

# Consumption Plan for Azure Functions
resource "azurerm_app_service_plan" "functions" {
  name                = "darksideleasing-demo-functions-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  kind                = "FunctionApp"
  reserved            = true
  sku {
    tier = "Dynamic"
    size = "Y1"
  }

  tags = {
    environment = "demo"
  }
}

# Azure Function App (HTTP API)
resource "azurerm_function_app" "http_api" {
  name                       = "darksideleasing-demo-http"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_app_service_plan.functions.id
  storage_account_name       = azurerm_storage_account.functions.name
  storage_account_access_key = azurerm_storage_account.functions.primary_access_key

  tags = {
    environment = "demo"
  }

  app_settings = {
    FUNCTIONS_WORKER_RUNTIME = "dotnet"
  }
}

# Azure Function App (Durable Queue)
resource "azurerm_function_app" "durable_queue" {
  name                       = "darksideleasing-demo-durable"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_app_service_plan.functions.id
  storage_account_name       = azurerm_storage_account.functions.name
  storage_account_access_key = azurerm_storage_account.functions.primary_access_key

  tags = {
    environment = "demo"
  }

  app_settings = {
    FUNCTIONS_WORKER_RUNTIME = "dotnet"
  }
}
