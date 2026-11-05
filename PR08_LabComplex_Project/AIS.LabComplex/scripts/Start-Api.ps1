$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
dotnet run --project src/AIS.API/AIS.API.csproj --launch-profile AIS.API
