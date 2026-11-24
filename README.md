# 📊 EnterpriseReportSystem (ASP.NET Core MVC)

Корпоративна уеб система за автоматизирано импортиране, валидиране, обработка и генериране на бизнес отчети от външни файлове, изградена върху **ASP.NET Core MVC** и **Entity Framework Core**.

> **🚧 Статус на проекта:** В активна разработка *(Work in Progress)* — основната слоеста архитектура, базата данни и моделите са изградени; предстои завършване на потребителския интерфейс, експорта и модулните тестове.

---

## 🏗️ Архитектура и Структура на Проекта

Проектът следва **Layered Architecture (Слоеста архитектура)** с ясно разделение на отговорностите:

* **`Controllers/`** — Обработка на HTTP заявките, маршрутизация и връзка със сервизния слой.
* **`Services/`** — Бизнес логика, парсване и валидация на входни файлове (`InputFiles/`), генериране на отчети.
* **`Data/` & `Migrations/`** — `ApplicationDbContext`, конфигурации на моделите (Fluent API) и Code-First миграции за SQL Server.
* **`Models/` & `ViewModels/`** — Домейн сущности (Entities) и специализирани ViewModels за сигурен трансфер на данни към изгледите.
* **`Common/`** — Общи константи, валидационни правила и помощни класове.
* **`Views/` & `wwwroot/`** — Razor изгледи, Bootstrap 5 стилизиране и клиентски ресурси.

---

## 🛠️ Използвани Технологии

* **Backend:** C# 12, .NET 8 / ASP.NET Core MVC
* **Database & ORM:** Microsoft SQL Server, Entity Framework Core (Code-First)
* **Frontend:** Razor Views, HTML5, CSS3, Bootstrap 5, JavaScript
* **Tools:** Git, GitHub, Visual Studio 2022

---

## 🗺️ План за Развитие (Roadmap)

- [x] Изграждане на слоеста архитектура (`Controllers`, `Services`, `Data`, `ViewModels`)
- [x] Проектиране на базата данни и EF Core миграции
- [x] Модул за зареждане и обработка на входни файлове (`InputFiles`)
- [ ] Разширена валидация и визуализация на грешките при импорт
- [ ] Генериране и експорт на обобщени отчети (Excel / PDF)
- [ ] Покритие с Unit и Integration тестове (xUnit / Moq)

---

## 🚀 Локално Стартиране

1. Клонирайте репозиторито:
   ```bash
   git clone https://github.com
   ```
2. Актуализирайте Connection String-а в `appsettings.json`, ако е необходимо.
3. Приложете миграциите към SQL Server:
   ```bash
   dotnet ef database update
   ```
4. Стартирайте приложението:
   ```bash
   dotnet run
   ```
