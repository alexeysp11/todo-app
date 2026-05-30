# Система управления задачами (WinForms / .NET Framework 4.8)

[English](README.md) | [Русский](README.ru.md)

Приложение для ведения списка дел (To-Do), разработанное на базе WinForms и PostgreSQL с использованием чистого ADO.NET.

## Технологический стек и архитектура

- **Слой UI**: WinForms (.NET Framework 4.8), использование Autofac для сборки форм.
- **Слой BLL**: Сервисы со строгой валидацией входных данных.
- **Слой DAL**: Чистые SQL-запросы через `Npgsql` (v4.1.14) с поддержкой async/await и `ConfigureAwait(false)`.
- **DI-контейнер**: Autofac с явным управлением областями видимости (`Lifetime Scope`) для дочерних окон.
- **Логирование**: Serilog с выводом в текстовые файлы и ежедневной ротацией.
- **Тестирование**: xUnit, Moq, AutoFixture и FluentAssertions.

---

## Развертывание базы данных

Локальная СУБД запускается через Docker Compose. Выполните команду в корневом каталоге проекта:

```bash
docker-compose up -d
```

Скрипт автоматически поднимает PostgreSQL на порту **54321** и инициализирует две изолированные базы данных:
1. `todo_management_db` — Рабочая база данных для самого приложения.
2. `todo_tests_db` — Изолированная песочница для интеграционных тестов.

---

## Конфигурация строк подключения

### Для приложения (`TodoApp.WinForms/App.config`)
```xml
<connectionStrings>
  <add name="TodoDbConnection" 
       connectionString="Server=localhost;Port=54321;Database=todo_management_db;User Id=postgres;Password=postgres;" 
       providerName="Npgsql" />
</connectionStrings>
```

### Для интеграционных тестов (`TodoApp.Tests.Integration/App.config`)
```xml
<connectionStrings>
  <add name="TodoDbConnection" 
       connectionString="Server=localhost;Port=54321;Database=todo_tests_db;User Id=postgres;Password=postgres;" 
       providerName="Npgsql" />
</connectionStrings>
```

---

## Запуск тестов

1. Скомпилируйте решение (`Ctrl + Shift + B`).
2. Откройте **Обозреватель тестов** (Test Explorer) в Visual Studio.
3. Запустите все тесты.

*Примечание: Интеграционные тесты автоматически очищают таблицы перед каждым прогоном через `TRUNCATE ... RESTART IDENTITY CASCADE` с помощью фикстур `IAsyncLifetime` в xUnit.*
