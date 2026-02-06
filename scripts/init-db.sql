-- Create schemas for each module
CREATE SCHEMA IF NOT EXISTS catalog;
CREATE SCHEMA IF NOT EXISTS orders;
CREATE SCHEMA IF NOT EXISTS basket;
CREATE SCHEMA IF NOT EXISTS customers;
CREATE SCHEMA IF NOT EXISTS identity;

-- Grant permissions
GRANT ALL ON SCHEMA catalog TO postgres;
GRANT ALL ON SCHEMA orders TO postgres;
GRANT ALL ON SCHEMA basket TO postgres;
GRANT ALL ON SCHEMA customers TO postgres;
GRANT ALL ON SCHEMA identity TO postgres;
