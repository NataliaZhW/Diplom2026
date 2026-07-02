# 🧵 Diplom_Winder

**Diplom_Winder** — это корпоративное веб-приложение для управления производственными заданиями мотальщиков. Система позволяет сотрудникам получать задания, запрашивать материалы, отслеживать статусы выполнения и вести учёт готовой продукции.

---

## 📋 Оглавление

- [Технологии](#технологии)
- [Архитектура](#архитектура)
- [Структура проекта](#структура-проекта)
- [База данных](#база-данных)
- [Установка и запуск](#установка-и-запуск)
- [Тестовые данные](#тестовые-данные)
- [API Эндпоинты](#api-эндпоинты)
- [Роли и права](#роли-и-права)

---

## 🛠 Технологии

### Бэкенд
| Технология | Версия | Назначение |
|------------|--------|------------|
| ASP.NET Core | 8.0 | Веб-фреймворк |
| C# | 12.0 | Язык программирования |
| Entity Framework Core | 8.0 | ORM для работы с БД |
| Pomelo.EntityFrameworkCore.MySql | 8.0 | Драйвер для MySQL |
| JWT Bearer | 8.0 | Аутентификация |
| BCrypt.Net-Next | 4.0.3 | Хеширование паролей |
| Swashbuckle | 6.5.0 | Swagger / OpenAPI |

### Фронтенд
| Технология | Версия | Назначение |
|------------|--------|------------|
| Vue.js | 3.5 | Фреймворк |
| Vite | 5.0 | Сборщик |
| Vue Router | 4.0 | Маршрутизация |
| Pinia | 2.0 | Управление состоянием |
| Axios | 1.6 | HTTP-запросы |

### Инфраструктура
| Технология | Версия | Назначение |
|------------|--------|------------|
| Docker | 24.0+ | Контейнеризация |
| MySQL | 8.0 | База данных |

---

## 🏗 Архитектура
┌─────────────────────────────────────────────────────────────┐
│ DOCKER │
├─────────────────────────────────────────────────────────────┤
│ ┌────────────┐ ┌────────────┐ ┌────────────┐ │
│ │ MySQL │ │ MySQL │ │ ASP.NET │ │
│ │ outside │ │ winder │ │ Core │ │
│ │ (порт │ │ (порт │ │ (порт │ │
│ │ 3306) │ │ 3307) │ │ 5217) │ │
│ └────────────┘ └────────────┘ └────────────┘ │
│ │ │ │ │
│ ▼ ▼ ▼ │
│ ┌────────────────────────────────────────────────┐ │
│ │ Vue.js 3 + Vite (порт 5173) │ │
│ └────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘

text

### Базы данных

| База данных | Назначение | Доступ |
|-------------|------------|--------|
| **db-outside** | Справочники (Users, ColorThreads, Icons, Kits, KitCompositions) | Только чтение |
| **db-winder** | Оперативные данные (BrandManufs) | Полный доступ |

---

## 📁 Структура проекта
Diplom_Winder/
├── .env # Переменные окружения (секреты)
├── .gitignore # Исключения для Git
├── docker-compose.yml # Оркестрация контейнеров
├── README.md # Описание проекта
│
├── init-scripts/ # SQL-скрипты для инициализации БД
│ ├── 01-outside.sql # Справочники
│ └── 02-winder.sql # Оперативные данные
│
├── BackendWinder/ # Бэкенд ASP.NET Core
│ ├── Program.cs # Точка входа
│ ├── appsettings.json # Конфигурация
│ ├── Models/ # Модели данных
│ ├── Data/ # Контексты БД
│ ├── DTOs/ # Объекты передачи данных
│ ├── Services/ # Бизнес-логика
│ └── Controllers/ # API-контроллеры
│
└── FrontendWinder/ # Фронтенд Vue.js
├── src/
│ ├── main.js # Точка входа
│ ├── App.vue # Корневой компонент
│ ├── router/ # Маршрутизация
│ ├── store/ # Управление состоянием
│ ├── api/ # HTTP-клиент
│ ├── components/ # Компоненты
│ ├── views/ # Страницы
│ └── styles/ # Глобальные стили
├── package.json # Зависимости
└── vite.config.js # Конфигурация сборки

text

---

## 🗄️ База данных

### db-outside (Справочники — ТОЛЬКО ЧТЕНИЕ)

| Таблица | Поля | Назначение |
|---------|------|------------|
| `Users` | id, login, password_hash, full_name, role, is_active, created_at | Пользователи системы |
| `ColorThreads` | id, code, name, pnk, dmc, created_at | Цвета ниток |
| `Icons` | id, icon | Значки для маркировки |
| `Kits` | id, internal_code, name, note | Наборы и схемы |
| `KitCompositions` | id, kit_id, icon_id, color_id, meterage, count_* | Составы наборов/схем |

### db-winder (Оперативные данные — ПОЛНЫЙ ДОСТУП)

| Таблица | Поля | Назначение |
|---------|------|------------|
| `BrandManufs` | id, code, name | Бренды производителей |

---

## 🚀 Установка и запуск

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (24.0+)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (8.0+)
- [Node.js](https://nodejs.org/) (18.0+)

### 1. Клонирование репозитория

```bash
git clone https://github.com/NataliaZhW/Diplom2026.git
cd Diplom2026
2. Создание файла .env
Создайте файл .env в корневой папке:

env
# Базы данных
DB_ROOT_PASSWORD=root123
DB_USER=diplom_user
DB_PASSWORD=Diplom_2026

# JWT настройки
JWT_SECRET=SuperSecretKeyForDiplomWinder2026!Min32Chars
JWT_ISSUER=diplom-winder-api
JWT_AUDIENCE=diplom-winder-client

# Окружение
ASPNETCORE_ENVIRONMENT=Development
3. Запуск баз данных (Docker)
bash
# Запуск контейнеров
docker-compose up -d

# Проверка статуса
docker-compose ps

# Проверка таблиц
docker exec -it db_outside mysql -uroot -proot123 -e "USE outside_db; SHOW TABLES;"
4. Запуск бэкенда
bash
cd BackendWinder
dotnet restore
dotnet run
Swagger: http://localhost:5217/swagger/index.html

5. Запуск фронтенда
bash
cd FrontendWinder
npm install
npm run dev
Приложение: http://localhost:5173

👤 Тестовые данные
Логин	Пароль	Роль
ivanov	password123	Мотальщик
petrov	password123	Мотальщик
sidorov	password123	Мотальщик
master1	password123	Мастер
admin	password123	Мастер
Тестовые комплекты (наборы/схемы)
Код	Название	Тип	Цветов	Каунты	Металлик	Перле
0048	Изумрудный город	Набор	16	283, 322	❌	❌
0037	С новым годом!	Набор	11	282, 283, 322	✅ M-02	✅ П-511
0269	Портрет Медведя	Схема	6	322	❌	❌
0123	Летний сад	Набор	17	283	❌	❌
0456	Осенний лес	Набор	6	283, 322	✅ M-T669	❌
0789	Морской бриз	Набор	5	322, 362	❌	✅ П-404
🌐 API Эндпоинты
Авторизация
Метод	Эндпоинт	Доступ	Назначение
POST	/api/Auth/login	Все	Вход, получение JWT
Справочники
Метод	Эндпоинт	Доступ	Назначение
GET	/api/Reference/colors	Все	Список цветов
GET	/api/Reference/brands	Все	Список брендов
GET	/api/Reference/users	Только мастер	Список пользователей
Каталог
Метод	Эндпоинт	Доступ	Назначение
GET	/api/Catalog/icons	Все	Список значков
GET	/api/Catalog/kits?type=kit	Все	Список наборов
GET	/api/Catalog/kits?type=scheme	Все	Список схем
GET	/api/Catalog/kits/{id}	Все	Детали комплекта
GET	/api/Catalog/threads?brand=dmc	Все	Список нитей
Пример запроса (вход)
bash
curl -X POST http://localhost:5217/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"login":"ivanov","password":"password123"}'
Пример ответа
json
{
  "userId": 1,
  "login": "ivanov",
  "fullName": "Иванов Иван Иванович",
  "role": "winder",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-06-29T..."
}
🔐 Роли и права
Роль	Доступ
Мотальщик (winder)	Вход, просмотр цветов, брендов, каталога, значков
Мастер (master)	Вход, просмотр цветов, брендов, каталога, значков, просмотр списка пользователей