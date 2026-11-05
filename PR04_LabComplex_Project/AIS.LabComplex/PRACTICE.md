# Практическая работа № 4

**Вариант:** № 1 — АИС «Лабораторный комплекс»

**Тема:** Проектирование контрактов данных и валидация авторизации

Это самостоятельный архив с полным исходным кодом учебного MVP.
Откройте `AIS.LabComplex.sln` в Visual Studio 2022 на Windows.
В этой версии выделены материалы, относящиеся к данной ПР:

- `src/AIS.API/DemoAuth.cs`
- `src/AIS.Core/LabService.cs`
- `docs/api/access-contract.md`

## Запуск

```powershell
dotnet run --project src/AIS.Demo/AIS.Demo.csproj
dotnet run --project tests/AIS.Tests/AIS.Tests.csproj
dotnet run --project src/AIS.API/AIS.API.csproj
```

Полная инструкция находится в `README.md`.
Применяется одна согласованная кодовая база для всех практических,
но `PRACTICE.md` выделяет предмет конкретной работы.
