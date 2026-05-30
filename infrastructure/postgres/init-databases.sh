#!/bin/bash
set -e

# Creating a database for tests
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE todo_tests_db;
EOSQL

# Roll out a single init.sql to the main database
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f /tmp/init.sql

# Roll out the same single init.sql to the test database
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "todo_tests_db" -f /tmp/init.sql
