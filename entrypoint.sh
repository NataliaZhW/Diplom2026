#!/bin/bash
# Ждём запуска MySQL
sleep 10

# Выполняем скрипт с принудительной кодировкой
mysql -u root -proot123 --default-character-set=utf8mb4 ReferenceDb < /docker-entrypoint-initdb.d/init.sql

# Запускаем обычный entrypoint
exec /entrypoint.sh mysqld