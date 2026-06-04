-- Установка кодировки
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

-- Создание таблиц
CREATE TABLE IF NOT EXISTS BrandManufs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(50) NOT NULL,
    Name VARCHAR(100) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS ColorThreads (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(50) NOT NULL,
    Name VARCHAR(100) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Заполнение тестовыми данными
INSERT INTO BrandManufs (Code, Name) VALUES 
('1', 'Gamma'),
('2', 'ПНК'),
('3', 'DMC'),
('П', 'ПЕРЛЕ'),
('М', 'Металлик');

INSERT INTO ColorThreads (Code, Name) VALUES 
('100', 'Белый'),
('101', 'Черный'),
('102', 'Серый'),
('103', 'Телесный светлый'),
('104', 'Черный с сединой'),
('105', 'Морковный крем'),
('106', 'Бледная поганка'),
('107', 'Хмурый рассвет'),
('108', 'Экрю'),
('109', 'Серебряный');