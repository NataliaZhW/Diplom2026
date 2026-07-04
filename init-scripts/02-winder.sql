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
    status ENUM('new','planned','materials_requested','materials_issued','in_progress','completed','accepted','archived') NOT NULL DEFAULT 'new',
    item_type ENUM('kit','scheme','thread') NOT NULL,
    item_id INT NOT NULL,
    item_code VARCHAR(20) NOT NULL,
    item_name VARCHAR(200) NOT NULL,
    brand_label VARCHAR(20) DEFAULT NULL,
    count_value INT DEFAULT NULL,
    quantity INT NOT NULL DEFAULT 1,
    winder_id INT NOT NULL,
    assigned_by INT DEFAULT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    assigned_at TIMESTAMP NULL DEFAULT NULL,
    materials_issued_at TIMESTAMP NULL DEFAULT NULL,
    completed_at TIMESTAMP NULL DEFAULT NULL,
    accepted_at TIMESTAMP NULL DEFAULT NULL,
    archived_at TIMESTAMP NULL DEFAULT NULL,
    note TEXT DEFAULT NULL,
    INDEX idx_tasks_winder (winder_id),
    INDEX idx_tasks_status (status),
    INDEX idx_tasks_item (item_type, item_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- ТЕСТОВЫЕ ДАННЫЕ: Tasks
-- ============================================================
INSERT INTO Tasks (
    status, item_type, item_id, item_code, item_name,
    brand_label, count_value, quantity,
    winder_id, assigned_by,
    created_at, assigned_at
) VALUES
('in_progress', 'scheme', 1, '0048', 'Изумрудный город', 'DMC', 283, 10, 1, 4, NOW(), NOW()),
('planned', 'scheme', 1, '0048', 'Изумрудный город', 'ПНК', 283, 15, 1, 4, NOW(), NOW()),
('new', 'kit', 1, '0048', 'Изумрудный город', NULL, NULL, 30, 1, NULL, NOW(), NULL),
('materials_issued', 'kit', 2, '0037', 'С новым годом!', NULL, NULL, 5, 2, 4, NOW(), NOW()),
('materials_requested', 'scheme', 3, '0269', 'Портрет Медведя', 'DMC', 322, 8, 2, 4, NOW(), NOW()),
('completed', 'scheme', 4, '0123', 'Летний сад', 'DMC', 283, 12, 3, 4, NOW(), NOW()),
('accepted', 'kit', 5, '0456', 'Осенний лес', NULL, NULL, 20, 3, 4, NOW(), NOW()),
('new', 'thread', 6, '0789', 'Морской бриз', 'DMC', NULL, 100, 1, NULL, NOW(), NULL),
('planned', 'thread', 6, '0789', 'Морской бриз', 'ПНК', NULL, 50, 2, 4, NOW(), NOW());

-- ============================================================
-- ПРОВЕРКА
-- ============================================================
SELECT 'Tables created and test data inserted!' as Status;
SELECT COUNT(*) as TasksCount FROM Tasks;
SELECT status, COUNT(*) as Count FROM Tasks GROUP BY status;