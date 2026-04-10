# MoneyManager

Мобильное Blazor-приложение для управления личным бюджетом с динамическим дневным лимитом.

## Архитектура — Clean Architecture

```
MoneyManager.sln
├── MoneyManager.Domain          # Entities, Domain Interfaces
├── MoneyManager.Application     # Use Cases, Services, DTOs, Commands
├── MoneyManager.Infrastructure  # EF Core, PostgreSQL, Repositories, BCrypt
└── MoneyManager/                # Blazor Server UI (mobile-first)
```

### Зависимости между слоями

```
Domain ← Application ← Infrastructure
                    ← Web (Presentation)
```

## Требования

- .NET 10 SDK
- PostgreSQL 14+
- dotnet-ef (для управления миграциями)

## Быстрый запуск

### 1. Настройте подключение к БД

Отредактируйте `MoneyManager/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=moneymanager;Username=postgres;Password=ВАШ_ПАРОЛЬ"
  }
}
```

### 2. Создайте базу данных

```bash
# Убедитесь, что PostgreSQL запущен
psql -U postgres -c "CREATE DATABASE moneymanager;"
```

### 3. Запустите приложение

```bash
cd MoneyManager
dotnet run
```

Миграции применяются **автоматически** при старте приложения.

### 4. Откройте в браузере

```
https://localhost:5001
```

Откройте в мобильном браузере или используйте DevTools → Toggle Device Toolbar (iPhone 14 Pro / 390px).

---

## Управление миграциями (вручную)

```bash
# Применить миграции
dotnet ef database update \
  --project MoneyManager.Infrastructure \
  --startup-project MoneyManager

# Создать новую миграцию
dotnet ef migrations add ИмяМиграции \
  --project MoneyManager.Infrastructure \
  --startup-project MoneyManager \
  --output-dir Persistence/Migrations

# Откатить последнюю
dotnet ef migrations remove \
  --project MoneyManager.Infrastructure \
  --startup-project MoneyManager
```

---

## Ключевые решения

### Почему нет DailyBudgetSnapshot?

Расчёт дневного лимита делается **на лету**:

```
todayLimit = (monthlyBudget - totalSpentSoFar) / daysRemainingIncludingToday
```

Для личного приложения этот запрос выполняется мгновенно — в таблице расходов сотни записей,
не миллионы. Snapshot-таблица добавила бы сложности (синхронизация, устаревание данных)
без реальной пользы. При необходимости аналитики она легко добавляется позже.

### Авторизация по слову-паролю

- Хранится хэш BCrypt (work factor 11) — невозможно восстановить исходное слово
- При логине: загружаем всех пользователей (их единицы) и проверяем `BCrypt.Verify()`
- В будущем: можно добавить полноценный JWT/Cookie auth

### Почему Blazor Server (не WebAssembly)?

- Серверный рендеринг: работает без JS-бандла, быстрее первый экран
- Прямой доступ к БД через DI без REST API
- Подходит для личного приложения с одним-двумя пользователями

### State Management

`UserSessionService` — scoped-сервис, живёт в рамках SignalR-соединения.
При закрытии вкладки — пользователь "выходит" (нет persistent cookie).
Это намеренно для упрощения. В продакшне: добавить Blazor Persistent State или cookie.

---

## Структура БД

```sql
users (id, display_name, access_key[bcrypt], created_at)
  └─ budget_plans (id, user_id, month, year, monthly_amount, created_at)
       └─ expenses (id, budget_plan_id, amount, category, note, expense_date, created_at)
```

Индексы:
- `users.access_key` — unique (быстрый поиск при логине)
- `budget_plans(user_id, year, month)` — unique (один план на месяц)
- `expenses(budget_plan_id, expense_date)` — для агрегации по датам

---

## Расширение в будущем

| Функция | Что нужно добавить |
|---|---|
| Несколько пользователей | Уже поддерживается (UserId везде присутствует) |
| Категории (кастомные) | Сущность `Category` + UI в настройках |
| Push-уведомления | Blazor PWA + Web Push API |
| Аналитика / графики | Компонент с Chart.js или ApexCharts |
| PWA-режим | `manifest.json` + `service-worker.js` в wwwroot |
| Несколько месяцев | `GetByUserAndMonthAsync` уже есть, нужен UI-выбор |
