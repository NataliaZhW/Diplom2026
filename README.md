справочники брендов и цветов нитей для вышивания (часть)

Технологии
- Backend: ASP.NET Core 9, Entity Framework Core
- Database: MySQL 8.0 (Docker)
- Frontend: Vue.js 3, Axios
- Deployment: Docker Compose

Требования
- Docker Desktop
- .NET 9 SDK
- Node.js 18+

Что работает:
✅ MySQL в Docker - база данных с правильной кодировкой
✅ ASP.NET Core API - бэкенд с CRUD операциями
✅ Vue.js фронтенд - интерфейс для управления справочниками
✅ Swagger - документация API
✅ Русские символы - корректное отображение

API Endpoints
GET /api/BrandManufs - получить все бренды
POST /api/BrandManufs - создать бренд
PUT /api/BrandManufs/{id} - обновить бренд
DELETE /api/BrandManufs/{id} - удалить бренд
Аналогично для /api/ColorThreads


Структура проекта:

E:\Diplom_Winder\
├── docker-compose.yml      # Оркестрация Docker
├── init.sql                # Инициализация БД
├── BackendWinder\          # ASP.NET Core API
│   ├── Controllers\        # BrandManufsController, ColorThreadsController
│   ├── Data\               # AppDbContext
│   ├── Models\             # BrandManuf, ColorThread
│   └── appsettings.json    # Настройки
└── FrontendWinder\         # Vue.js приложение
    ├── src\
    │   ├── api\            # Axios + API вызовы
    │   ├── components\     # BrandManufList, ColorThreadList
    │   └── App.vue
    └── package.json


Запустить powershell:

# База данных
cd E:\Diplom_Winder
docker-compose up -d

# Бэкенд
cd BackendWinder
dotnet run

# Фронтенд (новый терминал)
cd FrontendWinder
npm run dev

Остановить:
# Бэкенд: Ctrl+C
# Фронтенд: Ctrl+C
# База данных:
docker-compose down
