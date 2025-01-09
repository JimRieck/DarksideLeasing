variable "subscription_id" {}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

resource "azurerm_resource_group" "darkside_leasing_dev_east" {
  name     = "DarkSide_Leasing_Dev_East"
  location = "East US" # Change location if needed

  tags = {
    Environment = "Dev"
    Project     = "DarkSide Leasing"
  }
}
