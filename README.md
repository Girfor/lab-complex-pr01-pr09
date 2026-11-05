# АИС «Лабораторный комплекс» — практические работы №1–9

Учебные проекты на C#/.NET, вариант №1. Подготовлены для работы в Microsoft Visual Studio.

## Содержание

- [ПР1 — Анализ требований и структура репозитория](PR01_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР2 — Уровни интеграции и взаимодействие модулей](PR02_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР3 — Сопоставление данных (Data Mapping)](PR03_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР4 — Контракты данных и проверка авторизации](PR04_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР5 — Стратегия ветвления Git Flow](PR05_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР6 — Клиентская архитектура и OpenAPI](PR06_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР7 — Диаграмма модулей и структура проекта](PR07_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР8 — Артефакты и протоколы проекта](PR08_LabComplex_Project/AIS.LabComplex/PRACTICE.md)
- [ПР9 — .gitignore и права доступа](PR09_LabComplex_Project/AIS.LabComplex/PRACTICE.md)

Каждая практическая работа находится в собственной папке `PR01_LabComplex_Project` … `PR09_LabComplex_Project` и содержит исходный код, решение Visual Studio `AIS.LabComplex.sln`, тесты, документацию и описание соответствующего задания `PRACTICE.md`.

## Открытие и запуск

1. Установите Visual Studio с компонентами разработки C#/.NET и подходящий .NET SDK.
2. Откройте файл `AIS.LabComplex.sln` внутри папки выбранной практической работы.
3. Выполните восстановление NuGet-пакетов (Restore).
4. Выберите `AIS.API`, `AIS.UI` или `AIS.Demo` в зависимости от цели и запустите проект.
5. Тесты расположены в `tests/AIS.Tests`.

Функциональные разделы в `docs/`; вспомогательные PowerShell-скрипты в `scripts/`.

> Это учебные демонстрационные проекты. Учётные данные для разработки не следует использовать в production. Сборка в текущем окружении не выполнялась, так как отсутствует .NET SDK.
