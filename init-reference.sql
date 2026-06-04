-- =====================================================
-- init-reference.sql
-- БАЗА ДАННЫХ СПРАВОЧНИКОВ
-- =====================================================

USE ReferenceDb;

-- =====================================================
-- 1. БРЕНДЫ (производители ниток)
-- =====================================================
CREATE TABLE IF NOT EXISTS BrandManufs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO BrandManufs (Code, Name) VALUES 
('2', 'ПНК'),
('3', 'DMC')
ON DUPLICATE KEY UPDATE Name = VALUES(Name);

-- =====================================================
-- 2. ОСНОВНЫЕ ЦВЕТА НИТЕЙ (ColorThreads)
-- =====================================================
CREATE TABLE IF NOT EXISTS ColorThreads (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    FullName VARCHAR(200) NULL        -- Полное название с переводом
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO ColorThreads (Code, Name) VALUES 
('100', 'белый'),
('101', 'черный'),
('102', 'серый'),
('103', 'телесный светлый'),
('104', 'черный с сединой'),
('105', 'морковный крем'),
('106', 'бледная поганка'),
('107', 'хмурый рассвет'),
('108', 'экрю'),
('109', 'серебряный'),
('110', 'железный'),
('111', 'утренний иней'),
('112', 'грибной'),
('113', 'иссиня-черный'),
('114', 'телесный средний'),
('115', 'телесный темный'),
('116', 'телесный тусклый'),
('117', 'белый теплый'),
('118', 'голубь сизокрылый'),
('119', 'мышастый'),
('201', 'лесной орех'),
('202', 'пряничный домик'),
('203', 'совиный пух'),
('204', 'мореный дуб'),
('205', 'улиточный'),
('206', 'каштановый'),
('207', 'шоколадный'),
('208', 'корица'),
('209', 'красный кирпич'),
('210', 'терракотовый'),
('211', 'карамель'),
('212', 'медвежье ушко'),
('213', 'ушастая сова'),
('214', 'бронзовый канделябр'),
('301', 'лягушачья кожа'),
('302', 'зелёный дуб'),
('303', 'сухие травы'),
('304', 'нежная листва'),
('307', 'сочная зелень'),
('323', 'изумрудный город'),
('326', 'хвост ящерицы'),
('401', 'весенняя лазурь'),
('423', 'незабудка'),
('424', 'фиалка'),
('513', 'маков цвет'),
('515', 'коралловый'),
('603', 'медовый')
ON DUPLICATE KEY UPDATE 
    Name = VALUES(Name);

-- =====================================================
-- 3. НЕСТАНДАРТНЫЕ НИТИ: ПЕРЛЕ
-- =====================================================
CREATE TABLE IF NOT EXISTS PerleThreads (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO PerleThreads (Code, Name) VALUES 
('П-511', 'старые розы'),
('П-309', 'Трава-мурава'),
('П-404', 'Грозовое небо'),
('П-506', 'Яблоневый цвет'),
('П-704', 'Бутон пиона')
ON DUPLICATE KEY UPDATE Name = VALUES(Name);

-- =====================================================
-- 4. НЕСТАНДАРТНЫЕ НИТИ: МЕТАЛЛИК
-- =====================================================
CREATE TABLE IF NOT EXISTS MetallicThreads (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO MetallicThreads (Code, Name) VALUES 
('M-02', 'Gold'),
('M-T669', 'Green')
ON DUPLICATE KEY UPDATE Name = VALUES(Name);

-- =====================================================
-- 5. ТИПЫ КАУНТОВ (COUNT) С ЦВЕТОМ ЭТИКЕТКИ
-- =====================================================
CREATE TABLE IF NOT EXISTS CountTypes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code INT NOT NULL UNIQUE,               -- 252, 283, 282, 322 и т.д.
    DisplayName VARCHAR(50) NOT NULL,       -- '12,5(25)ct 2 нити'
    ThreadCount INT NOT NULL,               -- количество нитей: 2 или 3 или 1
    LabelColor VARCHAR(50) NOT NULL         -- цвет этикетки
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO CountTypes (Code, DisplayName, ThreadCount, LabelColor) VALUES 
(252, '12,5(25)ct 2 нити', 2, 'белый'),
(283, '14(28)ct 3 нити', 3, 'желтый'),
(282, '14(28)ct 2 нити', 2, 'оранжевый'),
(302, '15(30)ct 2 нити', 2, 'фиолетовый'),
(322, '16(32)ct 2 нити', 2, 'зеленый'),
(362, '18(36)ct 2 нити', 2, 'розовый'),
(402, '20(40)ct 1 нить', 1, 'голубой')
ON DUPLICATE KEY UPDATE 
    DisplayName = VALUES(DisplayName),
    ThreadCount = VALUES(ThreadCount),
    LabelColor = VALUES(LabelColor);

-- =====================================================
-- 6. ТИПЫ УПАКОВКИ (ПАКЕТЫ)
-- =====================================================
CREATE TABLE IF NOT EXISTS PackageTypes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO PackageTypes (Name) VALUES 
('Пакет для наборов маленький'),
('Пакет для наборов большой'),
('Пакет для схем 10х10'),
('Пакет для схем 12х15'),
('Пакет для схем 12х17'),
('Пакет для схем 15х20')
ON DUPLICATE KEY UPDATE Name = VALUES(Name);

-- =====================================================
-- 7. СОЕДИНЕНИЕ БРЕНДОВ И ЦВЕТОВ (БОБИНЫ И СВЯЗКИ)
-- =====================================================
-- Если сочетание есть в этой таблице -> "бобина"
-- Если нет -> "связка"
CREATE TABLE IF NOT EXISTS BrandColorBobbins (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    BrandManufId INT NOT NULL,
    ColorThreadId INT NOT NULL,
    BobbinType VARCHAR(20) DEFAULT 'бобина',
    
    FOREIGN KEY (BrandManufId) REFERENCES BrandManufs(Id),
    FOREIGN KEY (ColorThreadId) REFERENCES ColorThreads(Id),
    UNIQUE KEY UK_BrandColor (BrandManufId, ColorThreadId)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Вставляем бобины (ПНК белый/черный, DMC белый/черный/хвост ящерицы)
INSERT INTO BrandColorBobbins (BrandManufId, ColorThreadId) 
SELECT b.Id, c.Id
FROM BrandManufs b, ColorThreads c
WHERE (b.Code = '2' AND c.Code IN ('100', '101'))      -- ПНК белый, черный
   OR (b.Code = '3' AND c.Code IN ('100', '101', '326')) -- DMC белый, черный, хвост ящерицы
ON DUPLICATE KEY UPDATE BobbinType = 'бобина';

-- =====================================================
-- 8. КОМПЛЕКТЫ (СХЕМЫ И НАБОРЫ)
-- =====================================================
CREATE TABLE IF NOT EXISTS KitsSchemes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(200) NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

INSERT INTO KitsSchemes (Code, Name) VALUES 
('0037', 'С новым годом!'),
('0048', 'Изумрудный город'),
('0123', 'Летний сад'),
('0269', 'Портрет Медведя'),
('0456', 'Осенний лес'),
('0789', 'Морской бриз')
ON DUPLICATE KEY UPDATE Name = VALUES(Name);

-- =====================================================
-- 9. СОСТАВ КОМПЛЕКТА (НИТИ)
-- Три типа нитей: обычная (ColorThreads), перле, металлик
-- =====================================================
CREATE TABLE IF NOT EXISTS KitSchemeComposition (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KitSchemeId INT NOT NULL,
    ThreadType ENUM('Regular', 'Perle', 'Metallic') NOT NULL,
    RegularThreadId INT NULL,
    PerleThreadId INT NULL,
    MetallicThreadId INT NULL,
    BrandManufId INT NULL,
    
    FOREIGN KEY (KitSchemeId) REFERENCES KitsSchemes(Id) ON DELETE CASCADE,
    FOREIGN KEY (RegularThreadId) REFERENCES ColorThreads(Id),
    FOREIGN KEY (PerleThreadId) REFERENCES PerleThreads(Id),
    FOREIGN KEY (MetallicThreadId) REFERENCES MetallicThreads(Id),
    FOREIGN KEY (BrandManufId) REFERENCES BrandManufs(Id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 10. СХЕМЫ (варианты Count)
-- =====================================================
CREATE TABLE IF NOT EXISTS SchemeVariants (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KitSchemeId INT NOT NULL,
    CountTypeId INT NOT NULL,
    IsAvailable BOOLEAN DEFAULT TRUE,
    
    FOREIGN KEY (KitSchemeId) REFERENCES KitsSchemes(Id) ON DELETE CASCADE,
    FOREIGN KEY (CountTypeId) REFERENCES CountTypes(Id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 11. ДЕТАЛИ СХЕМ (количество пасм на каждый цвет)
-- =====================================================
CREATE TABLE IF NOT EXISTS SchemeVariantDetails (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SchemeVariantId INT NOT NULL,
    CompositionId INT NOT NULL,
    Quantity INT NOT NULL,
    
    FOREIGN KEY (SchemeVariantId) REFERENCES SchemeVariants(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompositionId) REFERENCES KitSchemeComposition(Id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 12. НАБОРЫ (варианты)
-- =====================================================
CREATE TABLE IF NOT EXISTS KitVariants (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KitSchemeId INT NOT NULL,
    IsAvailable BOOLEAN DEFAULT TRUE,
    
    FOREIGN KEY (KitSchemeId) REFERENCES KitsSchemes(Id) ON DELETE CASCADE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 13. ДЕТАЛИ НАБОРОВ (метраж на каждый цвет)
-- =====================================================
CREATE TABLE IF NOT EXISTS KitVariantDetails (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KitVariantId INT NOT NULL,
    CompositionId INT NOT NULL,
    Quantity INT NOT NULL,
    
    FOREIGN KEY (KitVariantId) REFERENCES KitVariants(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompositionId) REFERENCES KitSchemeComposition(Id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =====================================================
-- 14. ПЛАНОВЫЕ ЗАДАНИЯ
-- =====================================================
CREATE TABLE IF NOT EXISTS PlannedTasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    TaskType VARCHAR(20) NOT NULL,
    KitSchemeId INT NULL,
    SchemeVariantId INT NULL,
    KitVariantId INT NULL,
    BrandManufId INT NULL,
    ColorThreadId INT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    PackageTypeId INT NULL,
    ManualSpecification JSON NULL,
    Status VARCHAR(20) DEFAULT 'Draft',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    PublishedAt DATETIME NULL,
    
    FOREIGN KEY (KitSchemeId) REFERENCES KitsSchemes(Id),
    FOREIGN KEY (SchemeVariantId) REFERENCES SchemeVariants(Id),
    FOREIGN KEY (KitVariantId) REFERENCES KitVariants(Id),
    FOREIGN KEY (BrandManufId) REFERENCES BrandManufs(Id),
    FOREIGN KEY (ColorThreadId) REFERENCES ColorThreads(Id),
    FOREIGN KEY (PackageTypeId) REFERENCES PackageTypes(Id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE INDEX IX_PlannedTasks_Status ON PlannedTasks(Status);
CREATE INDEX IX_PlannedTasks_TaskType ON PlannedTasks(TaskType);

-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0048 "Изумрудный город"
-- =====================================================

-- Получаем ID комплекта 0048
SET @kitId = (SELECT Id FROM KitsSchemes WHERE Code = '0048');

-- 1. Вставляем состав комплекта (16 нитей: 15 обычных + 1 металлик)
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, MetallicThreadId, BrandManufId)
VALUES
(@kitId, 'Metallic', NULL, (SELECT Id FROM MetallicThreads WHERE Code = 'M-T669'), NULL),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '100'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '101'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '109'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '110'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '114'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '202'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '207'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '307'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '323'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '401'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '423'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '424'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '513'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '515'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')),
(@kitId, 'Regular', (SELECT Id FROM ColorThreads WHERE Code = '603'), NULL, (SELECT Id FROM BrandManufs WHERE Code = '3'));

-- 2. Вставляем варианты схем для 0048 (2 каунта: 283 и 322)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kitId, Id FROM CountTypes WHERE Code IN (283, 322);

-- 3. Вставляем количество пасм для схем (по вашим данным)
-- Сначала получим ID вариантов схем
SET @schemeVariant283 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kitId AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 283));
SET @schemeVariant322 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kitId AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 322));

-- ID нитей (сохраним для удобства)
SET @metalId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND ThreadType = 'Metallic');
SET @whiteId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '100'));
SET @blackId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '101'));
SET @silverId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '109'));
SET @ironId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '110'));
SET @nudeId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '114'));
SET @gingerId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '202'));
SET @chocolateId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '207'));
SET @greeneryId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '307'));
SET @emeraldId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '323'));
SET @azureId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '401'));
SET @forgetId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '423'));
SET @violetId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '424'));
SET @poppyId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '513'));
SET @coralId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '515'));
SET @honeyId = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kitId AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '603'));

-- Вставляем количество пасм для варианта 283
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@schemeVariant283, @metalId, 1),
(@schemeVariant283, @whiteId, 1),
(@schemeVariant283, @blackId, 1),
(@schemeVariant283, @silverId, 1),
(@schemeVariant283, @ironId, 1),
(@schemeVariant283, @nudeId, 1),
(@schemeVariant283, @gingerId, 1),
(@schemeVariant283, @chocolateId, 1),
(@schemeVariant283, @greeneryId, 2),
(@schemeVariant283, @emeraldId, 2),
(@schemeVariant283, @azureId, 1),
(@schemeVariant283, @forgetId, 1),
(@schemeVariant283, @violetId, 1),
(@schemeVariant283, @poppyId, 1),
(@schemeVariant283, @coralId, 1),
(@schemeVariant283, @honeyId, 1);

-- Вставляем количество пасм для варианта 322
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@schemeVariant322, @metalId, 1),
(@schemeVariant322, @whiteId, 1),
(@schemeVariant322, @blackId, 1),
(@schemeVariant322, @silverId, 1),
(@schemeVariant322, @ironId, 1),
(@schemeVariant322, @nudeId, 1),
(@schemeVariant322, @gingerId, 1),
(@schemeVariant322, @chocolateId, 1),
(@schemeVariant322, @greeneryId, 1),
(@schemeVariant322, @emeraldId, 1),
(@schemeVariant322, @azureId, 1),
(@schemeVariant322, @forgetId, 1),
(@schemeVariant322, @violetId, 1),
(@schemeVariant322, @poppyId, 1),
(@schemeVariant322, @coralId, 1),
(@schemeVariant322, @honeyId, 1);

-- 4. Вставляем вариант набора для 0048
INSERT INTO KitVariants (KitSchemeId) VALUES (@kitId);

SET @kitVariantId = (SELECT Id FROM KitVariants WHERE KitSchemeId = @kitId);

-- Вставляем метраж для набора (по вашим данным)
INSERT INTO KitVariantDetails (KitVariantId, CompositionId, Quantity) VALUES
(@kitVariantId, @metalId, 3),
(@kitVariantId, @whiteId, 6),
(@kitVariantId, @blackId, 4),
(@kitVariantId, @silverId, 8),
(@kitVariantId, @ironId, 4),
(@kitVariantId, @nudeId, 5),
(@kitVariantId, @gingerId, 6),
(@kitVariantId, @chocolateId, 5),
(@kitVariantId, @greeneryId, 10),
(@kitVariantId, @emeraldId, 10),
(@kitVariantId, @azureId, 4),
(@kitVariantId, @forgetId, 3),
(@kitVariantId, @violetId, 4),
(@kitVariantId, @poppyId, 4),
(@kitVariantId, @coralId, 4),
(@kitVariantId, @honeyId, 7);

-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0037 "С новым годом!"
-- =====================================================

SET @kit0037 = (SELECT Id FROM KitsSchemes WHERE Code = '0037');

-- 1. Состав комплекта: 11 обычных + 1 перле + 1 металлик
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, PerleThreadId, MetallicThreadId, BrandManufId)
SELECT @kit0037, 'Regular', Id, NULL, NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')
FROM ColorThreads WHERE Code IN ('100', '101', '102', '103', '108', '111', '201', '207', '301', '304', '326')
UNION ALL
SELECT @kit0037, 'Perle', NULL, Id, NULL, NULL
FROM PerleThreads WHERE Code IN ('П-511')
UNION ALL
SELECT @kit0037, 'Metallic', NULL, NULL, Id, NULL
FROM MetallicThreads WHERE Code IN ('M-02');

-- 2. Варианты схем: 3 каунта (283, 282, 322)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kit0037, Id FROM CountTypes WHERE Code IN (283, 282, 322);

-- 3. Количество пасм для схем (для каждого цвета)
SET @variant283 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0037 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 283));
SET @variant282 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0037 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 282));
SET @variant322 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0037 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 322));

-- ID нитей
SET @white0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '100'));
SET @black0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '101'));
SET @grey0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '102'));
SET @nudeLight0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '103'));
SET @ecru0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '108'));
SET @morningFrost0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '111'));
SET @hazelnut0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '201'));
SET @chocolate0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '207'));
SET @frogSkin0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '301'));
SET @tenderFoliage0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '304'));
SET @lizardTail0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '326'));
SET @perle0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND ThreadType = 'Perle');
SET @metallic0037 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0037 AND ThreadType = 'Metallic');

-- Данные для варианта 283
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant283, @white0037, 2),
(@variant283, @black0037, 1),
(@variant283, @grey0037, 1),
(@variant283, @nudeLight0037, 1),
(@variant283, @ecru0037, 2),
(@variant283, @morningFrost0037, 1),
(@variant283, @hazelnut0037, 2),
(@variant283, @chocolate0037, 1),
(@variant283, @frogSkin0037, 1),
(@variant283, @tenderFoliage0037, 2),
(@variant283, @lizardTail0037, 1),
(@variant283, @perle0037, 1),
(@variant283, @metallic0037, 1);

-- Данные для варианта 282
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant282, @white0037, 2),
(@variant282, @black0037, 1),
(@variant282, @grey0037, 1),
(@variant282, @nudeLight0037, 1),
(@variant282, @ecru0037, 1),
(@variant282, @morningFrost0037, 1),
(@variant282, @hazelnut0037, 1),
(@variant282, @chocolate0037, 1),
(@variant282, @frogSkin0037, 1),
(@variant282, @tenderFoliage0037, 1),
(@variant282, @lizardTail0037, 1),
(@variant282, @perle0037, 1),
(@variant282, @metallic0037, 1);

-- Данные для варианта 322
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant322, @white0037, 1),
(@variant322, @black0037, 1),
(@variant322, @grey0037, 1),
(@variant322, @nudeLight0037, 1),
(@variant322, @ecru0037, 1),
(@variant322, @morningFrost0037, 1),
(@variant322, @hazelnut0037, 1),
(@variant322, @chocolate0037, 1),
(@variant322, @frogSkin0037, 1),
(@variant322, @tenderFoliage0037, 1),
(@variant322, @lizardTail0037, 1),
(@variant322, @perle0037, 1),
(@variant322, @metallic0037, 1);

-- 4. Набор для 0037
INSERT INTO KitVariants (KitSchemeId) VALUES (@kit0037);
SET @kitVariant0037 = (SELECT Id FROM KitVariants WHERE KitSchemeId = @kit0037);

INSERT INTO KitVariantDetails (KitVariantId, CompositionId, Quantity) VALUES
(@kitVariant0037, @white0037, 5),
(@kitVariant0037, @black0037, 3),
(@kitVariant0037, @grey0037, 3),
(@kitVariant0037, @nudeLight0037, 3),
(@kitVariant0037, @ecru0037, 4),
(@kitVariant0037, @morningFrost0037, 3),
(@kitVariant0037, @hazelnut0037, 5),
(@kitVariant0037, @chocolate0037, 3),
(@kitVariant0037, @frogSkin0037, 3),
(@kitVariant0037, @tenderFoliage0037, 4),
(@kitVariant0037, @lizardTail0037, 2),
(@kitVariant0037, @perle0037, 2),
(@kitVariant0037, @metallic0037, 2);


-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0269 "Портрет Медведя"
-- =====================================================

SET @kit0269 = (SELECT Id FROM KitsSchemes WHERE Code = '0269');

-- 1. Состав: 6 обычных цветов, без перле и металлик
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, PerleThreadId, MetallicThreadId, BrandManufId)
SELECT @kit0269, 'Regular', Id, NULL, NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')
FROM ColorThreads WHERE Code IN ('101', '113', '204', '206', '207', '212');

-- 2. Вариант схемы: 1 каунт (322)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kit0269, Id FROM CountTypes WHERE Code = 322;

SET @variant0269 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0269);

-- ID нитей
SET @black0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '101'));
SET @blueBlack0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '113'));
SET @stainedOak0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '204'));
SET @chestnut0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '206'));
SET @chocolate0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '207'));
SET @bearEar0269 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0269 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '212'));

INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant0269, @black0269, 2),
(@variant0269, @blueBlack0269, 2),
(@variant0269, @stainedOak0269, 3),
(@variant0269, @chestnut0269, 2),
(@variant0269, @chocolate0269, 4),
(@variant0269, @bearEar0269, 2);

-- НЕТ НАБОРА (KitVariants не создаем)


-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0123 "Летний сад"
-- =====================================================

SET @kit0123 = (SELECT Id FROM KitsSchemes WHERE Code = '0123');

-- 1. Состав: 17 обычных цветов
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, PerleThreadId, MetallicThreadId, BrandManufId)
SELECT @kit0123, 'Regular', Id, NULL, NULL, (SELECT Id FROM BrandManufs WHERE Code = '2')
FROM ColorThreads WHERE Code IN ('100', '102', '105', '109', '118', '119', '202', '208', '209', '210', '211', '212', '301', '302', '303', '304', '326');

-- 2. Вариант схемы: 1 каунт (283)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kit0123, Id FROM CountTypes WHERE Code = 283;

SET @variant0123 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0123);

-- Количество пасм (для всех цветов 1, кроме белого и зеленых)
INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity)
SELECT @variant0123, Id, 
    CASE 
        WHEN RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '100') THEN 2
        WHEN RegularThreadId IN (SELECT Id FROM ColorThreads WHERE Code IN ('301', '302', '303', '304')) THEN 2
        ELSE 1
    END
FROM KitSchemeComposition WHERE KitSchemeId = @kit0123;

-- 3. Набор для 0123
INSERT INTO KitVariants (KitSchemeId) VALUES (@kit0123);
SET @kitVariant0123 = (SELECT Id FROM KitVariants WHERE KitSchemeId = @kit0123);

INSERT INTO KitVariantDetails (KitVariantId, CompositionId, Quantity)
SELECT @kitVariant0123, Id,
    CASE 
        WHEN RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '100') THEN 8
        WHEN RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '326') THEN 6
        ELSE 4
    END
FROM KitSchemeComposition WHERE KitSchemeId = @kit0123;


-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0456 "Осенний лес"
-- =====================================================

SET @kit0456 = (SELECT Id FROM KitsSchemes WHERE Code = '0456');

-- 1. Состав: 6 обычных + 1 металлик
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, PerleThreadId, MetallicThreadId, BrandManufId)
SELECT @kit0456, 'Regular', Id, NULL, NULL, (SELECT Id FROM BrandManufs WHERE Code = '3')
FROM ColorThreads WHERE Code IN ('105', '201', '205', '207', '208', '210')
UNION ALL
SELECT @kit0456, 'Metallic', NULL, NULL, Id, NULL
FROM MetallicThreads WHERE Code IN ('M-02');

-- 2. Варианты схем: 2 каунта (283 и 322)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kit0456, Id FROM CountTypes WHERE Code IN (283, 322);

SET @variant0456_283 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0456 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 283));
SET @variant0456_322 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0456 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 322));

SET @carrotCream0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '105'));
SET @hazelnut0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '201'));
SET @snail0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '205'));
SET @chocolate0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '207'));
SET @cinnamon0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '208'));
SET @terracotta0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '210'));
SET @gold0456 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0456 AND ThreadType = 'Metallic');

INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant0456_283, @carrotCream0456, 2),
(@variant0456_283, @hazelnut0456, 2),
(@variant0456_283, @snail0456, 1),
(@variant0456_283, @chocolate0456, 2),
(@variant0456_283, @cinnamon0456, 2),
(@variant0456_283, @terracotta0456, 1),
(@variant0456_283, @gold0456, 1);

INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant0456_322, @carrotCream0456, 1),
(@variant0456_322, @hazelnut0456, 1),
(@variant0456_322, @snail0456, 1),
(@variant0456_322, @chocolate0456, 1),
(@variant0456_322, @cinnamon0456, 1),
(@variant0456_322, @terracotta0456, 1),
(@variant0456_322, @gold0456, 1);

-- 3. Набор для 0456
INSERT INTO KitVariants (KitSchemeId) VALUES (@kit0456);
SET @kitVariant0456 = (SELECT Id FROM KitVariants WHERE KitSchemeId = @kit0456);

INSERT INTO KitVariantDetails (KitVariantId, CompositionId, Quantity) VALUES
(@kitVariant0456, @carrotCream0456, 5),
(@kitVariant0456, @hazelnut0456, 4),
(@kitVariant0456, @snail0456, 3),
(@kitVariant0456, @chocolate0456, 5),
(@kitVariant0456, @cinnamon0456, 4),
(@kitVariant0456, @terracotta0456, 3),
(@kitVariant0456, @gold0456, 2);


-- =====================================================
-- ЗАПОЛНЕНИЕ ДАННЫХ ДЛЯ КОМПЛЕКТА 0789 "Морской бриз"
-- =====================================================

SET @kit0789 = (SELECT Id FROM KitsSchemes WHERE Code = '0789');

-- 1. Состав: 5 обычных + 1 перле
INSERT INTO KitSchemeComposition (KitSchemeId, ThreadType, RegularThreadId, PerleThreadId, MetallicThreadId, BrandManufId)
SELECT @kit0789, 'Regular', Id, NULL, NULL, (SELECT Id FROM BrandManufs WHERE Code = '2')
FROM ColorThreads WHERE Code IN ('100', '101', '102', '103', '118')
UNION ALL
SELECT @kit0789, 'Perle', NULL, Id, NULL, NULL
FROM PerleThreads WHERE Code IN ('П-404');

-- 2. Варианты схем: 2 каунта (322 и 362)
INSERT INTO SchemeVariants (KitSchemeId, CountTypeId)
SELECT @kit0789, Id FROM CountTypes WHERE Code IN (322, 362);

SET @variant0789_322 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0789 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 322));
SET @variant0789_362 = (SELECT Id FROM SchemeVariants WHERE KitSchemeId = @kit0789 AND CountTypeId = (SELECT Id FROM CountTypes WHERE Code = 362));

SET @white0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '100'));
SET @black0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '101'));
SET @grey0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '102'));
SET @nude0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '103'));
SET @blueDove0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND RegularThreadId = (SELECT Id FROM ColorThreads WHERE Code = '118'));
SET @perle0789 = (SELECT Id FROM KitSchemeComposition WHERE KitSchemeId = @kit0789 AND ThreadType = 'Perle');

INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant0789_322, @white0789, 1),
(@variant0789_322, @black0789, 1),
(@variant0789_322, @grey0789, 1),
(@variant0789_322, @nude0789, 1),
(@variant0789_322, @blueDove0789, 2),
(@variant0789_322, @perle0789, 1);

INSERT INTO SchemeVariantDetails (SchemeVariantId, CompositionId, Quantity) VALUES
(@variant0789_362, @white0789, 1),
(@variant0789_362, @black0789, 1),
(@variant0789_362, @grey0789, 1),
(@variant0789_362, @nude0789, 1),
(@variant0789_362, @blueDove0789, 1),
(@variant0789_362, @perle0789, 1);

-- 3. Набор для 0789
INSERT INTO KitVariants (KitSchemeId) VALUES (@kit0789);
SET @kitVariant0789 = (SELECT Id FROM KitVariants WHERE KitSchemeId = @kit0789);

INSERT INTO KitVariantDetails (KitVariantId, CompositionId, Quantity) VALUES
(@kitVariant0789, @white0789, 4),
(@kitVariant0789, @black0789, 3),
(@kitVariant0789, @grey0789, 3),
(@kitVariant0789, @nude0789, 4),
(@kitVariant0789, @blueDove0789, 6),
(@kitVariant0789, @perle0789, 2);


-- =====================================================
-- СОЗДАЕМ ПОЛЬЗОВАТЕЛЯ ТОЛЬКО ДЛЯ ЧТЕНИЯ
-- =====================================================
CREATE USER IF NOT EXISTS 'readonly'@'%' IDENTIFIED BY 'readonly123';
GRANT SELECT ON ReferenceDb.* TO 'readonly'@'%';
FLUSH PRIVILEGES;

-- =====================================================
-- ФИНАЛЬНАЯ ПРОВЕРКА
-- =====================================================
SHOW TABLES;
SELECT 'Init completed!' AS Status;