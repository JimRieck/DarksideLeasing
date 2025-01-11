variable "subscription_id" {
  description = "The subscription ID for Azure resources"
  type        = string
}

variable "create_resources" {
  description = "Flag to determine if the resources should be created"
  type        = bool
  default     = true
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# Resource Group
resource "azurerm_resource_group" "main" {
  count    = var.create_resources ? 1 : 0
  name     = "darksideleasing-dev-AUSCEN"
  location = "Australia Central"
}

# App Service Plan
resource "azurerm_service_plan" "ui" {
  count               = var.create_resources ? 1 : 0
  name                = "darksideleasing-dev-ui-plan"
  location            = azurerm_resource_group.main[count.index].location
  resource_group_name = azurerm_resource_group.main[count.index].name
  os_type             = "Windows"
  sku_name            = "B1"  # Change to a different SKU, e.g., Free tier

  tags = {
    environment = "dev"
  }
}

# App Service for Blazor Server UI
resource "azurerm_windows_web_app" "ui" {
  count               = var.create_resources ? 1 : 0
  name                = "darksideleasing-dev-ui"
  location            = azurerm_resource_group.main[count.index].location
  resource_group_name = azurerm_resource_group.main[count.index].name
  service_plan_id     = azurerm_service_plan.ui[count.index].id

  site_config {
    always_on = true
  }

  tags = {
    environment = "dev"
  }
}

# Serverless SQL Server
resource "azurerm_mssql_server" "db" {
  count                        = var.create_resources ? 1 : 0
  name                         = "darksideleasing-dev-sql"
  location                     = azurerm_resource_group.main[count.index].location
  resource_group_name          = azurerm_resource_group.main[count.index].name
  administrator_login          = "sqladmin"
  administrator_login_password = "P@ssw0rd1234!"
  version                      = "12.0"

  tags = {
    environment = "dev"
  }
}

# Serverless SQL Database
resource "azurerm_mssql_database" "db" {
  count                        = var.create_resources ? 1 : 0
  name                         = "darksideleasing-dev-db"
  server_id                    = azurerm_mssql_server.db[count.index].id
  sku_name                     = "GP_S_Gen5_2"  # General Purpose, Gen5 with 2 vCores
  max_size_gb                  = 10             # Set a valid maximum size (e.g., 10 GB)
  min_capacity                 = 2  
  auto_pause_delay_in_minutes  = 60     # Set a valid auto pause delay value

  tags = {
    environment = "dev"
  }
}

# Storage Account for Functions
resource "azurerm_storage_account" "functions" {
  count                    = var.create_resources ? 1 : 0
  name                     = "darksideleasingdevfuncs"
  resource_group_name      = azurerm_resource_group.main[count.index].name
  location                 = azurerm_resource_group.main[count.index].location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = "dev"
  }
}

# HTTP Triggered Azure Function
resource "azurerm_function_app" "http_api" {
  count                      = var.create_resources ? 1 : 0
  name                       = "darksideleasing-dev-http-api"
  location                   = azurerm_resource_group.main[count.index].location
  resource_group_name        = azurerm_resource_group.main[count.index].name
  app_service_plan_id        = azurerm_service_plan.ui[count.index].id
  storage_account_name       = azurerm_storage_account.functions[count.index].name
  storage_account_access_key = azurerm_storage_account.functions[count.index].primary_access_key

  tags = {
    environment = "dev"
  }
}

# Durable Azure Function
resource "azurerm_function_app" "durable_queue" {
  count                      = var.create_resources ? 1 : 0
  name                       = "darksideleasing-dev-durable-queue"
  location                   = azurerm_resource_group.main[count.index].location
  resource_group_name        = azurerm_resource_group.main[count.index].name
  app_service_plan_id        = azurerm_service_plan.ui[count.index].id
  storage_account_name       = azurerm_storage_account.functions[count.index].name
  storage_account_access_key = azurerm_storage_account.functions[count.index].primary_access_key

  tags = {
    environment = "dev"
  }
}
