# DiplomWinder - Система управления заданиями для мотальщиков

Система для управления заданиями, материалами и отчетами в организации по производству нитей для вышивания.

## Технологии

- **Backend:** ASP.NET Core 9, Entity Framework Core, Identity, JWT
- **Database:** MySQL 8.0 (Docker) - две базы данных
- **Frontend:** Vue.js 3, Vue Router, Pinia, Axios
- **Deployment:** Docker Compose

## Требования

- Docker Desktop
- .NET 9 SDK
- Node.js 18+

## Функциональность

### Для мотальщика (роль User)
- ✅ Регистрация и авторизация (JWT)
- ✅ Просмотр своих заданий
- ✅ Создание новых заданий (ручные)
- ✅ Изменение статусов заданий
- ✅ Запрос материалов
- ✅ Просмотр запросов материалов

### Для администратора (роль Admin)
- ✅ Управление пользователями
- ✅ Просмотр всех заданий
- ✅ Назначение/удаление роли Admin
- ✅ Удаление заданий

## Базы данных

| База данных | Порт | Назначение | Доступ |
|-------------|------|------------|--------|
| ReferenceDb | 3307 | Справочники (бренды, цвета, схемы, наборы) | Readonly |
| AppDb | 3308 | Оперативные данные (пользователи, задания, запросы) | Полный |

## Структура проекта
Diplom_Winder/
├── docker-compose.yml # Оркестрация Docker
├── init-reference.sql # Инициализация справочной БД
├── init-app.sql # Инициализация оперативной БД
│
├── BackendWinder/ # ASP.NET Core API
│ ├── Controllers/
│ │ ├── AuthController.cs # Регистрация, логин, профиль
│ │ ├── ReferenceController.cs # Справочники
│ │ ├── TasksController.cs # Задания мотальщика
│ │ ├── MaterialsController.cs # Запросы материалов
│ │ └── AdminController.cs # Управление пользователями
│ ├── Data/ # Контексты БД и модели
│ ├── Models/ # DTO
│ ├── Services/ # JwtService
│ └── appsettings.json # Конфигурация
│
└── FrontendWinder/ # Vue.js приложение
├── src/
│ ├── services/ # API сервисы
│ ├── stores/ # Pinia хранилища
│ ├── views/ # Страницы
│ │ ├── LoginView.vue
│ │ ├── RegisterView.vue
│ │ ├── HomeView.vue
│ │ ├── CreateTaskView.vue
│ │ ├── MaterialsView.vue
│ │ └── AdminView.vue
│ └── components/ # Компоненты
└── package.json


## API Endpoints

### Auth (`/api/Auth`)
| Метод | Эндпоинт | Назначение |
|-------|----------|------------|
| POST | `/register` | Регистрация |
| POST | `/login` | Вход |
| GET | `/profile` | Профиль |
| POST | `/change-password` | Смена пароля |

### Reference (`/api/Reference`)
| Метод | Эндпоинт | Назначение |
|-------|----------|------------|
| GET | `/brands` | Бренды |
| GET | `/colors` | Цвета |
| GET | `/count-types` | Каунты |
| GET | `/kits-schemes` | Комплекты |

### Tasks (`/api/Tasks`)
| Метод | Эндпоинт | Назначение |
|-------|----------|------------|
| GET | `/my` | Мои задания |
| POST | `/` | Создать задание |
| PUT | `/{id}/status` | Изменить статус |
| POST | `/{id}/request-materials` | Запросить материалы |

### Materials (`/api/Materials`)
| Метод | Эндпоинт | Назначение |
|-------|----------|------------|
| GET | `/requests/my` | Мои запросы |
| POST | `/requests` | Создать запрос |
| POST | `/requests/receive` | Подтвердить получение |

### Admin (`/api/Admin`) - только для Admin
| Метод | Эндпоинт | Назначение |
|-------|----------|------------|
| GET | `/users` | Все пользователи |
| POST | `/users/role` | Изменить роль |
| GET | `/tasks` | Все задания |
| DELETE | `/tasks/{id}` | Удалить задание |

## Запуск проекта

### 1. Запуск баз данных (Docker)

```powershell
cd E:\Diplom_Winder
docker-compose up -d

### 2. Запуск бэкенда

```powershell
cd BackendWinder
dotnet run

### 3. Запуск фронтенда (в новом терминале)

```powershell
cd FrontendWinder
npm install
npm run dev

### Остановка

```powershell
# Остановка бэкенда: Ctrl+C
# Остановка фронтенда: Ctrl+C

# Остановка баз данных
docker-compose down

### доступен:
Бэкенд будет доступен: http://localhost:5281
Swagger документация: http://localhost:5281/swagger
Фронтенд будет доступен: http://localhost:5173
