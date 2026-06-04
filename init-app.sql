-- =====================================================
-- init-app.sql
-- БАЗА ДАННЫХ ОПЕРАТИВНЫХ ДАННЫХ
-- =====================================================
-- Эта база хранит данные, которые меняются в процессе работы:
-- - профили пользователей (ФИО)
-- - задания мотальщиков
-- - запросы материалов
-- =====================================================

USE AppDb;

-- =====================================================
-- 1. ПРОФИЛИ ПОЛЬЗОВАТЕЛЕЙ
-- =====================================================
-- Хранит дополнительную информацию о пользователях.
-- Основная таблица пользователей (AspNetUsers) создается автоматически 
-- системой ASP.NET Core Identity. Связь через поле UserId.
-- =====================================================
CREATE TABLE IF NOT EXISTS UserProfiles (
    Id INT AUTO_INCREMENT PRIMARY KEY,          -- Уникальный ID записи
    UserId VARCHAR(255) NOT NULL UNIQUE,        -- GUID пользователя из AspNetUsers
    FullName VARCHAR(200) NOT NULL,             -- Полное имя (ФИО)
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP -- Дата создания профиля
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 2. ЗАДАНИЯ МОТАЛЬЩИКОВ
-- =====================================================
-- Хранит задания, выданные мотальщикам в работу.
-- PlannedTaskId = NULL означает, что задание создано вручную (не из плана)
-- =====================================================
CREATE TABLE IF NOT EXISTS AssignedTasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Ссылка на плановое задание (из ReferenceDb)
    -- NULL = задание создано вручную, не из плана
    PlannedTaskId INT NULL,
    
    -- Кому выдано (GUID мотальщика из AspNetUsers)
    AssignedToUserId VARCHAR(255) NOT NULL,
    
    -- Тип задания: 'Scheme' (схема), 'Kit' (набор), 'Thread' (нитки)
    TaskType VARCHAR(20) NOT NULL,
    
    -- Данные задания (копируются из ReferenceDb или вводятся вручную)
    KitSchemeCode VARCHAR(20) NULL,      -- Код комплекта ('0048', '0037')
    KitSchemeName VARCHAR(200) NULL,     -- Название комплекта ('Изумрудный город')
    BrandCode VARCHAR(20) NULL,          -- Бренд ('2'=ПНК, '3'=DMC)
    ColorCode VARCHAR(20) NULL,          -- Код цвета ('100', '202')
    CountCode INT NULL,                  -- Код каунта (283, 322, 282 и т.д.)
    
    -- Количество (штук, метров или пасм - зависит от TaskType)
    Quantity INT NOT NULL DEFAULT 1,
    
    -- Примечание (например: 'магазин', 'оптовик', 'выставка', 'на стенд')
    Notes TEXT NULL,
    
    -- Статус задания (цепочка переходов)
    Status VARCHAR(30) DEFAULT 'New',
    
    -- Даты ключевых событий
    AssignedAt DATETIME DEFAULT CURRENT_TIMESTAMP,  -- Дата выдачи задания
    MaterialsRequestedAt DATETIME NULL,              -- Дата запроса материалов
    MaterialsReceivedAt DATETIME NULL,               -- Дата получения материалов
    SubmittedAt DATETIME NULL,                       -- Дата сдачи готовой работы
    ReportedAt DATETIME NULL,                        -- Дата внесения в отчетность
    ArchivedAt DATETIME NULL                         -- Дата архивации
    
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 3. ЗАПРОСЫ МАТЕРИАЛОВ
-- =====================================================
-- Хранит запросы материалов для заданий.
-- Один запрос может относиться к нескольким заданиям (поле TaskIds JSON)
-- Может быть дозапросом (поле Notes = 'дозапрос')
-- =====================================================
CREATE TABLE IF NOT EXISTS MaterialRequests (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Список ID заданий, для которых запрашиваются материалы
    -- Пример: [101, 105, 203]
    TaskIds JSON NOT NULL,
    
    -- Кто запросил материалы (GUID мотальщика из AspNetUsers)
    RequestedByUserId VARCHAR(255) NOT NULL,
    
    -- Список запрашиваемых материалов в формате JSON
    -- Структура описана ниже
    Materials JSON NOT NULL,
    
    -- Статус запроса
    Status VARCHAR(20) DEFAULT 'Pending',  -- 'Pending', 'Received', 'Cancelled'
    
    -- Даты
    RequestedAt DATETIME DEFAULT CURRENT_TIMESTAMP,  -- Дата запроса
    ReceivedAt DATETIME NULL,                        -- Дата получения материалов
    
    -- Примечание (например: 'дозапрос', 'срочно')
    Notes TEXT NULL
    
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- СТРУКТУРА JSON ПОЛЕЙ
-- =====================================================

/*
Materials (поле в таблице MaterialRequests) - формат JSON:

{
  "threads": [
    {"brandCode": "3", "colorCode": "100", "quantity": 6},
    {"brandCode": "3", "colorCode": "101", "quantity": 4},
    {"brandCode": "Perle", "colorCode": "П-511", "quantity": 2},
    {"brandCode": "Metallic", "colorCode": "M-02", "quantity": 1}
  ],
  "labels": [
    {"color": "желтый", "quantity": 10},
    {"color": "зеленый", "quantity": 5}
  ],
  "packages": [
    {"type": "Пакет для схем 10х10", "quantity": 8}
  ],
  "tags": true,      -- true = бирочки нужны, false = не нужны
  "twine": 2         -- количество отрезков шпагата (0 = не нужен)
}

Пояснение полей:
- threads    : список ниток (обычные, перле, металлик)
- labels     : этикетки по цветам
- packages   : упаковка по типам
- tags       : нужны ли бирочки (true/false)
- twine      : количество отрезков шпагата (целое число)
*/

-- =====================================================
-- СТАТУСЫ ЗАДАНИЙ (цепочка)
-- =====================================================
/*
New              → Новое (только создано)
MaterialsRequested → Материалы запрошены (мотальщик отправил запрос)
MaterialsReceived  → Материалы получены (склад выдал)
InProgress       → В работе (мотальщик начал выполнение)
Submitted        → Сдано (готово, ждет приемки)
Reported         → Внесено в отчетность (принято, учтено)
Archived         → В архиве (закрыто, неактивно)
*/

-- =====================================================
-- ФИНАЛЬНАЯ ПРОВЕРКА
-- =====================================================
SHOW TABLES;
SELECT 'Init-app completed!' AS Status;