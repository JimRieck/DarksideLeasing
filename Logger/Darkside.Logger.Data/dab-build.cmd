@echo off
@echo This cmd file creates a Data API Builder configuration based on the chosen database objects.
@echo To run the cmd, create an .env file with the following contents:
@echo dab-connection-string=your connection string
@echo ** Make sure to exclude the .env file from source control **
@echo **
dotnet tool install -g Microsoft.DataApiBuilder
dab init -c dab-config.json --database-type mssql --connection-string "@env('dab-connection-string')" --host-mode Development
@echo Adding tables
dab add "BillingHistory" --source "[dbo].[BillingHistory]" --fields.include "BillingId,TenantId,PeriodStart,PeriodEnd,AmountBilled,DocumentCount,PaymentStatus,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "DocumentRequest" --source "[dbo].[DocumentRequests]" --fields.include "RequestId,TemplateId,RequestedByUserId,RequestedOn,Status,GeneratedUrl,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "FieldValue" --source "[dbo].[FieldValues]" --fields.include "FieldValueId,RequestId,FieldName,Value,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "FlywaySchemaHistory" --source "[dbo].[flyway_schema_history]" --fields.include "installed_rank,version,description,type,script,checksum,installed_by,installed_on,execution_time,success" --permissions "anonymous:*" 
dab add "Log" --source "[dbo].[Logs]" --fields.include "Id,Timestamp,LogLevel,Application,Module,TenantId,Message,Exception,Properties" --permissions "anonymous:*" 
dab add "Payment" --source "[dbo].[Payments]" --fields.include "PaymentId,BillingId,PaymentDate,Amount,Provider,TransactionId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "Plan" --source "[dbo].[Plans]" --fields.include "PlanId,Name,PricePerMonth,MaxDocuments,MaxUsers,IsActive,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "Template" --source "[dbo].[Templates]" --fields.include "TemplateId,TenantId,Name,StorageUrl,FieldMetadataJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "Tenant" --source "[dbo].[Tenants]" --fields.include "TenantId,TenantName,IsActive,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "TenantSubscription" --source "[dbo].[TenantSubscriptions]" --fields.include "SubscriptionId,TenantId,PlanId,StartDate,EndDate,IsActive,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
dab add "User" --source "[dbo].[Users]" --fields.include "UserId,TenantId,Email,Role,IsActive,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn" --permissions "anonymous:*" 
@echo Adding views and tables without primary key
@echo Adding relationships
dab update BillingHistory --relationship Tenant --target.entity Tenant --cardinality one
dab update Tenant --relationship BillingHistory --target.entity BillingHistory --cardinality many
dab update DocumentRequest --relationship User --target.entity User --cardinality one
dab update User --relationship DocumentRequest --target.entity DocumentRequest --cardinality many
dab update DocumentRequest --relationship Template --target.entity Template --cardinality one
dab update Template --relationship DocumentRequest --target.entity DocumentRequest --cardinality many
dab update FieldValue --relationship DocumentRequest --target.entity DocumentRequest --cardinality one
dab update DocumentRequest --relationship FieldValue --target.entity FieldValue --cardinality many
dab update Payment --relationship BillingHistory --target.entity BillingHistory --cardinality one
dab update BillingHistory --relationship Payment --target.entity Payment --cardinality many
dab update Template --relationship Tenant --target.entity Tenant --cardinality one
dab update Tenant --relationship Template --target.entity Template --cardinality many
dab update TenantSubscription --relationship Plan --target.entity Plan --cardinality one
dab update Plan --relationship TenantSubscription --target.entity TenantSubscription --cardinality many
dab update TenantSubscription --relationship Tenant --target.entity Tenant --cardinality one
dab update Tenant --relationship TenantSubscription --target.entity TenantSubscription --cardinality many
dab update User --relationship Tenant --target.entity Tenant --cardinality one
dab update Tenant --relationship User --target.entity User --cardinality many
@echo Adding stored procedures
@echo **
@echo ** run 'dab validate' to validate your configuration **
@echo ** run 'dab start' to start the development API host **
