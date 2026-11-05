$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
if (-not (Test-Path '.git')) { git init -b main }
git status --short
if (Get-Command git-lfs -ErrorAction SilentlyContinue) {
    git lfs install --local
    git lfs track '*.pdf' '*.zip'
    git lfs ls-files
} else {
    Write-Host 'Git LFS не найден. Установите Git LFS отдельно.'
}
