-- ============================================================
-- БАЗА ДАННЫХ: winder_db (Оперативные данные - ПОЛНЫЙ ДОСТУП)
-- ============================================================

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

CREATE DATABASE IF NOT EXISTS winder_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE winder_db;

-- ============================================================
-- ТАБЛИЦА 1: BrandManufs (бренды производителей)
-- ============================================================
CREATE TABLE IF NOT EXISTS BrandManufs (
    id INT PRIMARY KEY AUTO_INCREMENT,
    code VARCHAR(20) UNIQUE NOT NULL COMMENT 'Код бренда',
    name VARCHAR(100) NOT NULL COMMENT 'Название бренда'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX idx_brands_code ON BrandManufs(code);
CREATE INDEX idx_brands_name ON BrandManufs(name);

INSERT INTO BrandManufs (code, name) VALUES
('2', 'ПНК'),
('3', 'DMC');

-- ============================================================
-- ТАБЛИЦА 2: Tasks (задания)
-- ============================================================
CREATE TABLE IF NOT EXISTS Tasks (
    id INT PRIMARY KEY AUTO_INCREMENT,
    status ENUM(
        'new',
        'materials_requested',
        'materials_received',
        'submitted',
        'reported',
        'archived'
    ) NOT NULL DEFAULT 'new' COMMENT 'Статус задания',

    -- Данные из "Выбрано"
    item_type ENUM('kit', 'scheme', 'thread') NOT NULL COMMENT 'Тип: набор/схема/нить',
    item_id INT NOT NULL COMMENT 'ID из Kits или ColorThreads',
    item_code VARCHAR(20) NOT NULL COMMENT 'Код (например, 0048)',
    item_name VARCHAR(200) NOT NULL COMMENT 'Название',
    brand_label VARCHAR(20) DEFAULT NULL COMMENT 'ПНК/DMC/ПЕРЛЕ/Металлик',
    count_value INT DEFAULT NULL COMMENT 'Каунт (для схем)',
    quantity INT NOT NULL DEFAULT 1 COMMENT 'Количество',
    
    -- Кто и когда
    winder_id INT NOT NULL COMMENT 'Мотальщик (кому назначено)',
    assigned_by INT DEFAULT NULL COMMENT 'Кем назначено/согласовано (мастер)',
    
    -- Даты
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Когда создано',
    assigned_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда назначено/согласовано',
    materials_requested_at  TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда материалы запрошены',
    materials_issued_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда материалы выданы',
    submitted_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда сдано',
    accepted_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда принято (мастером)',
    reported_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда внесено в отчетность',
    archived_at TIMESTAMP NULL DEFAULT NULL COMMENT 'Когда отправлено в архив',
    
    -- Примечания
    note TEXT DEFAULT NULL COMMENT 'Примечание',
    
    -- Индексы
    INDEX idx_tasks_winder (winder_id),
    INDEX idx_tasks_status (status),
    INDEX idx_tasks_item (item_type, item_id),
    INDEX idx_tasks_created (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- ТЕСТОВЫЕ ДАННЫЕ: Tasks
-- ============================================================
INSERT INTO Tasks (
    status, item_type, item_id, item_code, item_name,
    brand_label, count_value, quantity,
    winder_id, assigned_by,
    created_at, assigned_at,
    materials_issued_at, submitted_at, accepted_at, reported_at, archived_at
) VALUES
-- 1. Иванов — Изумрудный город (Схема, DMC, 283, 10 шт)
('materials_received', 'scheme', 1, '0048', 'Изумрудный город', 'DMC', 283, 10, 1, 4, 
 NOW(), NOW(), NOW(), NULL, NULL, NULL, NULL),

-- 2. Иванов — Изумрудный город (Схема, ПНК, 283, 15 шт)
('materials_requested', 'scheme', 1, '0048', 'Изумрудный город', 'ПНК', 283, 15, 1, 4, 
 NOW(), NOW(), NULL, NULL, NULL, NULL, NULL),

-- 3. Иванов — Изумрудный город (Набор, 30 шт)
('new', 'kit', 1, '0048', 'Изумрудный город', NULL, NULL, 30, 1, NULL, 
 NOW(), NULL, NULL, NULL, NULL, NULL, NULL),

-- 4. Петров — С новым годом! (Набор, 5 шт) — материалы выданы
('materials_received', 'kit', 2, '0037', 'С новым годом!', NULL, NULL, 5, 2, 4, 
 '2026-07-01 14:00:00', '2026-07-01 14:30:00', '2026-07-02 10:00:00', NULL, NULL, NULL, NULL),

-- 5. Петров — Портрет Медведя (Схема, DMC, 322, 8 шт)
('materials_requested', 'scheme', 3, '0269', 'Портрет Медведя', 'DMC', 322, 8, 2, 4, 
 '2026-07-02 11:00:00', '2026-07-02 11:30:00', NULL, NULL, NULL, NULL, NULL),

-- 6. Сидоров — Летний сад (Схема, DMC, 283, 12 шт) — сдано, но не принято
('submitted', 'scheme', 4, '0123', 'Летний сад', 'DMC', 283, 12, 3, 4, 
 '2026-06-28 09:00:00', '2026-06-28 10:00:00', '2026-06-29 10:00:00', '2026-07-01 16:00:00', NULL, NULL, NULL),

-- 7. Сидоров — Осенний лес (Набор, 20 шт) — принято (есть дата)
('submitted', 'kit', 5, '0456', 'Осенний лес', NULL, NULL, 20, 3, 4, 
 '2026-06-27 08:00:00', '2026-06-27 09:00:00', '2026-06-28 10:00:00', '2026-06-30 15:00:00', 
 '2026-07-01 12:00:00', NULL, NULL),

-- 8. Иванов — Морской бриз (Нить, DMC, 100 шт) — новое
('new', 'thread', 6, '0789', 'Морской бриз', 'DMC', NULL, 100, 1, NULL, 
 '2026-07-03 13:00:00', NULL, NULL, NULL, NULL, NULL, NULL),

-- 9. Петров — Морской бриз (Нить, ПНК, 50 шт) — в архиве
('archived', 'thread', 6, '0789', 'Морской бриз', 'ПНК', NULL, 50, 2, 4, 
 '2026-07-02 15:00:00', '2026-07-02 15:30:00', NULL, NULL, NULL, NULL, '2026-07-04 10:00:00'),

-- 10. Петров — С новым годом! (Схема, DMC, 282, 3 шт) — внесено в отчетность
('reported', 'scheme', 2, '0037', 'С новым годом!', 'DMC', 282, 3, 2, 4, 
 '2026-07-01 09:00:00', '2026-07-01 09:30:00', '2026-07-01 11:00:00', '2026-07-02 12:00:00', 
 '2026-07-02 14:00:00', '2026-07-03 09:00:00', NULL);
 
-- ============================================================
-- ПРОВЕРКА
-- ============================================================
SELECT 'Tables created and test data inserted!' as Status;
SELECT COUNT(*) as TasksCount FROM Tasks;
SELECT status, COUNT(*) as Count FROM Tasks GROUP BY status;