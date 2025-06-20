-- Update test user with BCrypt password hash for Test123!
-- This is a BCrypt hash of "Test123!" generated with work factor 10
UPDATE "Users" 
SET "Password" = '$2a$10$K7L0a2QrPUhPeW3qLJHqVeKxqWlHvHZLRxAi0OlQs7i7KHgCkwWka'
WHERE "Email" = 'test@example.com';

-- Check the update
SELECT "Id", "Email", "Password" FROM "Users" WHERE "Email" = 'test@example.com';