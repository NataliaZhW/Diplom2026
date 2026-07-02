-- ============================================================
-- БАЗА ДАННЫХ: outside_db (Справочники - ТОЛЬКО ЧТЕНИЕ)
-- ============================================================

-- ============================================================
-- Устанавливаем кодировку UTF-8
-- ============================================================
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

CREATE DATABASE IF NOT EXISTS outside_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE outside_db;

-- ============================================================
-- ТАБЛИЦА 1: Users
-- ============================================================
CREATE TABLE IF NOT EXISTS Users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    role ENUM('winder', 'master') NOT NULL DEFAULT 'winder',
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- ТАБЛИЦА 2: ColorThreads (цвета ниток)
-- ============================================================
CREATE TABLE IF NOT EXISTS ColorThreads (
    id INT PRIMARY KEY AUTO_INCREMENT,
    code VARCHAR(10) UNIQUE NOT NULL COMMENT 'Код цвета',
    name VARCHAR(100) NOT NULL COMMENT 'Название цвета',
    pnk VARCHAR(10) DEFAULT NULL COMMENT 'Соответствие для бренда ПНК',
    dmc VARCHAR(10) DEFAULT NULL COMMENT 'Соответствие для бренда DMC',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX idx_colors_code ON ColorThreads(code);
CREATE INDEX idx_colors_name ON ColorThreads(name);

-- ============================================================
-- ЗАПОЛНЯЕМ ТЕСТОВЫМИ ДАННЫМИ
-- ============================================================

-- Пароль для всех пользователей: "password123"
-- Правильный BCrypt хеш (work factor = 12)
SET @password_hash = '$2a$11$VU5LuMRDLEzXjC3j9vl.luyQDl8nfOY3qcpSBLTP/jvwvzrqNM7mK';

INSERT INTO Users (login, password_hash, full_name, role) VALUES
('ivanov', @password_hash, 'Иванов Иван Иванович', 'winder'),
('petrov', @password_hash, 'Петров Петр Петрович', 'winder'),
('sidorov', @password_hash, 'Сидоров Сидор Сидорович', 'winder'),
('master1', @password_hash, 'Мастерова Анна Сергеевна', 'master'),
('admin', @password_hash, 'Администратор Системы', 'master');

-- ЦВЕТА
INSERT INTO ColorThreads (code, name, pnk, dmc) VALUES
('104', 'черный с сединой', 'С', 'С'),
('105', 'морковный крем', 'С', 'С'),
('106', 'бледная поганка', 'С', 'С'),
('107', 'хмурый рассвет', 'С', 'С'),
('109', 'серебряный', 'С', 'С'),
('110', 'железный', 'С', 'С'),
('112', 'грибной', 'С', 'С'),
('113', 'иссиня-черный', 'С', 'С'),
('118', 'голубь сизокрылый', 'С', 'С'),
('119', 'мышастый', 'С', 'С'),
('201', 'лесной орех', 'С', 'С'),
('202', 'пряничный домик', 'С', 'С'),
('203', 'совиный пух', 'С', 'С'),
('204', 'мореный дуб', 'С', 'С'),
('205', 'улиточный', 'С', 'С'),
('206', 'каштановый', 'С', 'С'),
('207', 'шоколадный', 'С', 'С'),
('208', 'корица', 'С', 'С'),
('209', 'красный кирпич', 'С', 'С'),
('210', 'терракотовый', 'С', 'С'),
('211', 'карамель', 'С', 'С'),
('212', 'медвежье ушко', 'С', 'С'),
('213', 'ушастая сова', 'С', 'С'),
('214', 'бронзовый канделябр', 'С', 'С'),
('301', 'лягушачья кожа', 'С', 'С'),
('302', 'зелёный дуб', 'С', 'С'),
('303', 'сухие травы', 'С', 'С'),
('304', 'нежная листва', 'С', 'С'),
('100', 'белый', 'Б', 'Б'),
('101', 'черный', 'Б', 'Б'),
('102', 'серый', 'Б', 'Б'),
('103', 'телесный светлый', 'Б', 'Б'),
('108', 'экрю', 'Б', 'Б'),
('111', 'утренний иней', 'Б', 'Б'),
('114', 'телесный средний', 'Б', 'Б'),
('115', 'телесный темный', 'Б', 'Б'),
('116', 'телесный тусклый', 'Б', 'Б'),
('117', 'белый теплый', '-', 'Б'),
('326', 'хвост ящерицы', 'С', 'Б'),
('П-511', 'старые розы', 'П', 'П'),
('П-309', 'Трава-мурава', 'П', 'П'),
('П-404', 'Грозовое небо', 'П', 'П'),
('П-506', 'Яблоневый цвет', 'П', 'П'),
('П-704', 'Бутон пиона', 'П', 'П'),
('M-02', 'Gold', 'М', 'М'),
('M-T669', 'Green', 'М', 'М');

-- ПРОВЕРКА
SELECT 'Tables created and test data inserted!' as Status;
SELECT COUNT(*) as UsersCount FROM Users;
SELECT COUNT(*) as ColorsCount FROM ColorThreads;