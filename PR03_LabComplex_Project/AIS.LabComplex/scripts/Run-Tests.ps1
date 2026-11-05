$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
dotnet run --project tests/AIS.Tests/AIS.Tests.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
