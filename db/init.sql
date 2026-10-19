-- Runs once, when the db volume is first created (DP-02).
-- The development database (POSTGRES_DB) is created by the image; this adds the test database.
CREATE DATABASE "TaskDb_Test";
