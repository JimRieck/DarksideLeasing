provider "azurerm" {
  features {}
  subscription_id = "efc18ccd-6b22-4dd0-8fb5-a628aa8609a6" # Replace with your subscription ID
}

resource "azurerm_resource_group" "darkside_leasing_dev_east" {
  name     = "DarkSide_Leasing_Dev_East"
  location = "East US" # Change location if needed

  tags = {
    Environment = "Dev"
    Project     = "DarkSide Leasing"
  }
}