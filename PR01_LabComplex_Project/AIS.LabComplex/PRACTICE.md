# Практическая работа № 1

**Вариант:** № 1 — АИС «Лабораторный комплекс»

**Тема:** Анализ требований и первичное проектирование структуры репозитория

Это самостоятельный архив с полным исходным кодом учебного MVP.
Откройте `AIS.LabComplex.sln` в Visual Studio 2022 на Windows.
В этой версии выделены материалы, относящиеся к данной ПР:

- `src/AIS.Core/LabModels.cs`
- `src/AIS.Core/LabService.cs`
- `docs/requirements/requirements.md`

## Запуск

```powershell
dotnet run --project src/AIS.Demo/AIS.Demo.csproj
dotnet run --project tests/AIS.Tests/AIS.Tests.csproj
dotnet run --project src/AIS.API/AIS.API.csproj
```

Полная инструкция находится в `README.md`.
Применяется одна согласованная кодовая база для всех практических,
но `PRACTICE.md` выделяет предмет конкретной работы.
