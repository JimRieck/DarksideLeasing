# Define variables
$containerName = "sqldb"
$saPassword = "MyStrongP@ssw0rd123!"
$stage1Path = "C:\Code\MyStuff\DarksideLeasing\Logger\flyway\sql\stage1"
$stage2Path = "C:\Code\MyStuff\DarksideLeasing\Logger\flyway\sql\stage2"
$flywayImage = "flyway/flyway"

# Kill and delete the container first
Write-Host "Removing any existing SQL Server container..."
docker rm -f $containerName 2>$null

# Delete the volume just in case (only if you use one — otherwise skip)
# docker volume rm mssqlvolume 2>$null

# Start fresh
Write-Host "Starting new SQL Server container..."
docker run -d --name $containerName `
    -e "ACCEPT_EULA=Y" `
    -e "SA_PASSWORD=$saPassword" `
    -p 1433:1433 `
    mcr.microsoft.com/mssql/server:2022-latest

Write-Host "Waiting for SQL Server to be ready..."
Start-Sleep -Seconds 10

# OPTIONAL: Confirm password works
Write-Host "🔍 Testing SQL Server SA login..."
docker run -it --rm mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd `
    -S host.docker.internal,1433 -U sa -P "$saPassword" `
    -Q "SELECT name FROM sys.databases"

# Run Flyway Stage 1
Write-Host "🚀 Running Flyway Stage 1 (master DB)..."
docker run --rm --network="host" `
    -v "${stage1Path}:/flyway/sql" `
    $flywayImage migrate `
    -url="jdbc:sqlserver://host.docker.internal:1433;databaseName=master;encrypt=true;trustServerCertificate=true" `
    -user="sa" `
    -password="$saPassword" `
    -baselineOnMigrate=true

# Run Flyway Stage 2
Write-Host "🚀 Running Flyway Stage 2 (Darkside_Logging DB)..."
docker run --rm --network="host" `
    -v "${stage2Path}:/flyway/sql" `
    $flywayImage migrate `
    -url="jdbc:sqlserver://host.docker.internal:1433;databaseName=Darkside_Logging;encrypt=true;trustServerCertificate=true" `
    -user="sa" `
    -password="$saPassword" `
    -baselineOnMigrate=true

Write-Host "✅ All done!"
