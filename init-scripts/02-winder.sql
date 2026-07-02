-- ============================================================
-- БАЗА ДАННЫХ: winder_db (Оперативные данные - ПОЛНЫЙ ДОСТУП)
-- ============================================================

-- ============================================================
-- Устанавливаем кодировку UTF-8
-- ============================================================
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

CREATE DATABASE IF NOT EXISTS winder_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE winder_db;

-- ============================================================
-- ТАБЛИЦА 1: BrandManufs (бренды производителей)
-- ============================================================
-- Назначение: справочник брендов
-- Доступ: ПОЛНЫЙ ДОСТУП
-- ============================================================
CREATE TABLE IF NOT EXISTS BrandManufs (
    id INT PRIMARY KEY AUTO_INCREMENT,
    code VARCHAR(20) UNIQUE NOT NULL COMMENT 'Код бренда',
    name VARCHAR(100) NOT NULL COMMENT 'Название бренда'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Создаем индексы для быстрого поиска
CREATE INDEX idx_brands_code ON BrandManufs(code);
CREATE INDEX idx_brands_name ON BrandManufs(name);

-- ============================================================
-- ЗАПОЛНЯЕМ ТЕСТОВЫМИ ДАННЫМИ (бренды)
-- ============================================================
INSERT INTO BrandManufs (code, name) VALUES
('2', 'ПНК'),
('3', 'DMC');

-- ============================================================
-- ПРОВЕРКА
-- ============================================================
SELECT 'Table BrandManufs created and test data inserted!' as Status;
SELECT COUNT(*) as BrandsCount FROM BrandManufs;