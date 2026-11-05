# АИС «Лабораторный комплекс» — вариант 1

Учебный проект на C#/.NET 8: партии реактивов, прием/списание,
бронирование оборудования, проверка пересечений, права доступа,
интеграционное сопоставление данных, HTTP API и WPF/MVVM-клиент.

## Требования

- Windows 10/11, .NET 8 SDK и Visual Studio 2022 (рабочая нагрузка .NET Desktop Development)
- Внешние NuGet-пакеты не нужны
- WPF запускается только на Windows

## Быстрый запуск

1. Откройте `AIS.LabComplex.sln` в Visual Studio.
2. Для простого консольного примера: запустите `AIS.Demo`.
3. Для API: запустите `AIS.API` (адрес `http://localhost:5080`).
4. В другом PowerShell запустите `AIS.UI`; кнопка «Обновить» загрузит реестр.
5. Для тестов: `dotnet run --project tests/AIS.Tests/AIS.Tests.csproj`.

Команды из PowerShell в корне распакованного проекта:

```powershell
dotnet build AIS.LabComplex.sln
dotnet run --project src/AIS.Demo/AIS.Demo.csproj
dotnet run --project src/AIS.API/AIS.API.csproj
dotnet run --project tests/AIS.Tests/AIS.Tests.csproj
```

## Проверка API

После старта API в новом PowerShell:

```powershell
Invoke-RestMethod http://localhost:5080/health
$h = @{ 'X-Demo-Key' = 'local-admin-example-2026' }
Invoke-RestMethod http://localhost:5080/api/v1/equipment -Headers $h
$body = @{ id = 'EQ-0042'; name = 'Микроскоп' } | ConvertTo-Json
Invoke-RestMethod http://localhost:5080/api/v1/equipment -Headers $h `
  -Method Post -ContentType 'application/json' -Body $body
```

Тестовые ключи в `src/AIS.API/appsettings.Development.json` различают
Administrator, LaboratoryAssistant, Researcher. Они намеренно предназначены
**только для локальной учебной демонстрации**: сервер принимает их лишь в
Development с loopback-адреса. Не публикуйте этот API в интернете.
Для производственного применения нужны OIDC/OAuth2, TLS, БД, миграции,
резервное копирование и эксплуатационные испытания.

## Ограничения MVP

- Данные хранятся в памяти API и сбрасываются после перезапуска.
- Доступ к оборудованию логически проверяется, физическое реле не управляется.
- Отдельный интегратор снабжения представлен JSON DTO и маппингом, без ERP.
- В `AIS.Tests` используются простые проверки без внешнего тестового фреймворка.
- Реальная сборка этого архива в контейнере не подтверждена: .NET SDK отсутствует;
  код и конфигурация проверены статически.
