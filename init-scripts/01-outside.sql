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
('323', 'Изумрудный город', 'С', 'С'),
('326', 'хвост ящерицы', 'С', 'Б'),
('П-511', 'старые розы', 'П', 'П'),
('П-309', 'Трава-мурава', 'П', 'П'),
('П-404', 'Грозовое небо', 'П', 'П'),
('П-506', 'Яблоневый цвет', 'П', 'П'),
('П-704', 'Бутон пиона', 'П', 'П'),
('M-02', 'Gold', 'М', 'М'),
('M-T669', 'Green', 'М', 'М');

-- ============================================================
-- ТАБЛИЦА 3: Icons (значки)
-- ============================================================
CREATE TABLE IF NOT EXISTS Icons (
    id INT PRIMARY KEY AUTO_INCREMENT,
    icon VARCHAR(20) NOT NULL UNIQUE COMMENT 'Значок с названием (например, ✔️Галочка)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Заполняем значками
INSERT INTO Icons (icon) VALUES
('✔️Галочка'),
('❄️Снежинка'),
('🔷Ромб'),
('🔺Треугольник'),
('🏈Косая'),
('➖Минус'),
('〰️Волна'),
('⚫Точка'),
('🍃Листик'),
('🌲Елочка'),
('⬆️Стрелочка'),
('❌Крестик'),
('⭕Кружок'),
('🧡Сердечко'),
('🏵️Цветочек'),
('⭐Звезда'),
('⬜ Квадратик'),
('0 Ноль'),
('1 Один'),
('2 Два'),
('3 Три'),
('4 Четыре'),
('5 Пять'),
('6 Шесть'),
('7 Семь'),
('8 Восемь'),
('9 Девять');

-- ============================================================
-- ТАБЛИЦА 4: Kits (наборы/схемы)
-- ============================================================
CREATE TABLE IF NOT EXISTS Kits (
    id INT PRIMARY KEY AUTO_INCREMENT,
    internal_code VARCHAR(20) UNIQUE NOT NULL COMMENT 'Внутренний номер (например, 0048)',
    name VARCHAR(200) NOT NULL COMMENT 'Название',
    note TEXT DEFAULT NULL COMMENT 'Примечание'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- ТАБЛИЦА 5: KitCompositions (составы наборов/схем)
-- ============================================================
CREATE TABLE IF NOT EXISTS KitCompositions (
    id INT PRIMARY KEY AUTO_INCREMENT,
    kit_id INT NOT NULL COMMENT 'Ссылка на Kit',
    icon_id INT NOT NULL COMMENT 'Ссылка на Icons',
    color_id INT NOT NULL COMMENT 'Ссылка на ColorThreads',
    meterage DECIMAL(10, 2) DEFAULT NULL COMMENT 'Метраж для набора',
    count_252 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 252',
    count_272 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 272',
    count_283 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 283',
    count_282 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 282',
    count_302 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 302',
    count_322 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 322',
    count_362 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 362',
    count_401 INT DEFAULT 0 COMMENT 'Количество пасм для каунта 401',
    note TEXT DEFAULT NULL COMMENT 'Пояснения',
    FOREIGN KEY (kit_id) REFERENCES Kits(id) ON DELETE CASCADE,
    FOREIGN KEY (icon_id) REFERENCES Icons(id),
    FOREIGN KEY (color_id) REFERENCES ColorThreads(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Создаем индексы
CREATE INDEX idx_kits_code ON Kits(internal_code);
CREATE INDEX idx_kits_name ON Kits(name);
CREATE INDEX idx_kitcomp_kit ON KitCompositions(kit_id);
CREATE INDEX idx_kitcomp_color ON KitCompositions(color_id);

-- ============================================================
-- ТЕСТОВЫЕ ДАННЫЕ: Kits и KitCompositions
-- ============================================================

-- Сначала убедимся, что все нужные цвета есть
INSERT IGNORE INTO ColorThreads (code, name, pnk, dmc) VALUES
('307', 'Сочная зелень', 'С', 'С'),
('401', 'Весенняя лазурь', 'С', 'С'),
('423', 'Незабудка', 'С', 'С'),
('424', 'Фиалка', 'С', 'С'),
('513', 'Маков цвет', 'С', 'С'),
('515', 'Коралловый', 'С', 'С'),
('603', 'Медовый', 'С', 'С');

-- ============================================================
-- 1. НАБОР: 0048-Изумрудный город
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0048', 'Изумрудный город', 'Набор с метражом и каунтами');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_283, count_322) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '✔️Галочка'), (SELECT id FROM ColorThreads WHERE code = 'M-T669'), 3, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❄️Снежинка'), (SELECT id FROM ColorThreads WHERE code = '100'), 6, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🔷Ромб'), (SELECT id FROM ColorThreads WHERE code = '101'), 4, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🔺Треугольник'), (SELECT id FROM ColorThreads WHERE code = '109'), 8, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🏈Косая'), (SELECT id FROM ColorThreads WHERE code = '110'), 27, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '➖Минус'), (SELECT id FROM ColorThreads WHERE code = '114'), 10, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '〰️Волна'), (SELECT id FROM ColorThreads WHERE code = '202'), 6, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⚫Точка'), (SELECT id FROM ColorThreads WHERE code = '207'), 5, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🍃Листик'), (SELECT id FROM ColorThreads WHERE code = '307'), 12, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🌲Елочка'), (SELECT id FROM ColorThreads WHERE code = '323'), 20, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⬆️Стрелочка'), (SELECT id FROM ColorThreads WHERE code = '401'), 18, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❌Крестик'), (SELECT id FROM ColorThreads WHERE code = '423'), 3, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭕Кружок'), (SELECT id FROM ColorThreads WHERE code = '424'), 9, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🧡Сердечко'), (SELECT id FROM ColorThreads WHERE code = '513'), 14, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🏵️Цветочек'), (SELECT id FROM ColorThreads WHERE code = '515'), 16, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭐Звезда'), (SELECT id FROM ColorThreads WHERE code = '603'), 24, 1, 1);

-- ============================================================
-- 2. НАБОР: 0037-С новым годом!
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0037', 'С новым годом!', 'Набор с метражом, каунтами, металликом и перле');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_282, count_283, count_322) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '〰️Волна'), (SELECT id FROM ColorThreads WHERE code = 'M-02'), 8, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🏵️Цветочек'), (SELECT id FROM ColorThreads WHERE code = 'П-511'), 6, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '✔️Галочка'), (SELECT id FROM ColorThreads WHERE code = '100'), 5, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❄️Снежинка'), (SELECT id FROM ColorThreads WHERE code = '101'), 4, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭐Звезда'), (SELECT id FROM ColorThreads WHERE code = '109'), 6, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🔷Ромб'), (SELECT id FROM ColorThreads WHERE code = '201'), 3, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🧡Сердечко'), (SELECT id FROM ColorThreads WHERE code = '207'), 4, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🌲Елочка'), (SELECT id FROM ColorThreads WHERE code = '301'), 8, 2, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❌Крестик'), (SELECT id FROM ColorThreads WHERE code = '302'), 6, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭕Кружок'), (SELECT id FROM ColorThreads WHERE code = '304'), 4, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🍃Листик'), (SELECT id FROM ColorThreads WHERE code = '108'), 5, 1, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⬆️Стрелочка'), (SELECT id FROM ColorThreads WHERE code = '118'), 3, 1, 1, 1);

-- ============================================================
-- 3. СХЕМА: 0269-Портрет Медведя (БЕЗ МЕТРАЖА)
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0269', 'Портрет Медведя', 'СХЕМА без метража, только пасмы (каунт 322)');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_322) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '1 Один'), (SELECT id FROM ColorThreads WHERE code = '100'), NULL, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '2 Два'), (SELECT id FROM ColorThreads WHERE code = '101'), NULL, 3),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '3 Три'), (SELECT id FROM ColorThreads WHERE code = '114'), NULL, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '4 Четыре'), (SELECT id FROM ColorThreads WHERE code = '207'), NULL, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '5 Пять'), (SELECT id FROM ColorThreads WHERE code = '302'), NULL, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '6 Шесть'), (SELECT id FROM ColorThreads WHERE code = '107'), NULL, 1);

-- ============================================================
-- 4. НАБОР: 0123-Летний сад
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0123', 'Летний сад', 'Набор с метражом и каунтами');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_283) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '✔️Галочка'), (SELECT id FROM ColorThreads WHERE code = '100'), 8, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❄️Снежинка'), (SELECT id FROM ColorThreads WHERE code = '101'), 4, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🔷Ромб'), (SELECT id FROM ColorThreads WHERE code = '103'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🔺Треугольник'), (SELECT id FROM ColorThreads WHERE code = '108'), 9, 3),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🏈Косая'), (SELECT id FROM ColorThreads WHERE code = '109'), 3, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '➖Минус'), (SELECT id FROM ColorThreads WHERE code = '111'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '〰️Волна'), (SELECT id FROM ColorThreads WHERE code = '114'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⚫Точка'), (SELECT id FROM ColorThreads WHERE code = '201'), 9, 3),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🍃Листик'), (SELECT id FROM ColorThreads WHERE code = '202'), 3, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🌲Елочка'), (SELECT id FROM ColorThreads WHERE code = '207'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⬆️Стрелочка'), (SELECT id FROM ColorThreads WHERE code = '208'), 3, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '❌Крестик'), (SELECT id FROM ColorThreads WHERE code = '209'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭕Кружок'), (SELECT id FROM ColorThreads WHERE code = '211'), 3, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🧡Сердечко'), (SELECT id FROM ColorThreads WHERE code = '301'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '🏵️Цветочек'), (SELECT id FROM ColorThreads WHERE code = '302'), 3, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⭐Звезда'), (SELECT id FROM ColorThreads WHERE code = '303'), 6, 2),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '⬜ Квадратик'), (SELECT id FROM ColorThreads WHERE code = '304'), 6, 2);

-- ============================================================
-- 5. НАБОР: 0456-Осенний лес
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0456', 'Осенний лес', 'Набор с метражом, каунтами и металликом');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_283, count_322) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '6 Шесть'), (SELECT id FROM ColorThreads WHERE code = 'M-T669'), 6, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '1 Один'), (SELECT id FROM ColorThreads WHERE code = '104'), 4, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '2 Два'), (SELECT id FROM ColorThreads WHERE code = '110'), 8, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '3 Три'), (SELECT id FROM ColorThreads WHERE code = '202'), 4, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '4 Четыре'), (SELECT id FROM ColorThreads WHERE code = '204'), 8, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '5 Пять'), (SELECT id FROM ColorThreads WHERE code = '302'), 4, 1, 1);

-- ============================================================
-- 6. НАБОР: 0789-Морской бриз
-- ============================================================
INSERT INTO Kits (internal_code, name, note) VALUES
('0789', 'Морской бриз', 'Набор с метражом, каунтами и перле');

INSERT INTO KitCompositions (kit_id, icon_id, color_id, meterage, count_322, count_362) VALUES
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '4 Четыре'), (SELECT id FROM ColorThreads WHERE code = 'П-404'), 5, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '0 Ноль'), (SELECT id FROM ColorThreads WHERE code = '100'), 6, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '1 Один'), (SELECT id FROM ColorThreads WHERE code = '101'), 3, 1, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '2 Два'), (SELECT id FROM ColorThreads WHERE code = '102'), 6, 2, 1),
(LAST_INSERT_ID(), (SELECT id FROM Icons WHERE icon = '3 Три'), (SELECT id FROM ColorThreads WHERE code = '111'), 3, 1, 1);

-- ПРОВЕРКА
SELECT 'Tables created and test data inserted!' as Status;
SELECT COUNT(*) as UsersCount FROM Users;
SELECT COUNT(*) as ColorsCount FROM ColorThreads;