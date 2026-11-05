# Практическая работа № 8

**Вариант:** № 1 — АИС «Лабораторный комплекс»

**Тема:** Разработка перечня артефактов и протоколов проекта

Это самостоятельный архив с полным исходным кодом учебного MVP.
Откройте `AIS.LabComplex.sln` в Visual Studio 2022 на Windows.
В этой версии выделены материалы, относящиеся к данной ПР:

- `docs/artifacts.md`
- `docs/adr/ADR-001.md`
- `scripts/Generate-SAT.ps1`

## Запуск

```powershell
dotnet run --project src/AIS.Demo/AIS.Demo.csproj
dotnet run --project tests/AIS.Tests/AIS.Tests.csproj
dotnet run --project src/AIS.API/AIS.API.csproj
```

Полная инструкция находится в `README.md`.
Применяется одна согласованная кодовая база для всех практических,
но `PRACTICE.md` выделяет предмет конкретной работы.
