param([string]$Output = 'docs/SAT-protocol.md')
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$date = (Get-Date).ToString('yyyy-MM-dd HH:mm')
$text = @"
# Протокол SAT — АИС «Лабораторный комплекс»

Дата проверки: $date

| Проверка | Результат | Примечание |
|---|---|---|
| Создание партии реактива | Требует проверки | |
| Отказ на двойное бронирование | Требует проверки | |
| Блокировка неисправного прибора | Требует проверки | |
| Доступ по ролям | Требует проверки | |

Подпись проверяющего: __________________
"@
New-Item -ItemType Directory -Force -Path (Split-Path $Output) | Out-Null
Set-Content -Path $Output -Value $text -Encoding UTF8
Write-Host "Создан документ $Output"
