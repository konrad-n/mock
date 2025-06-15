-- Create test user for E2E tests if not exists
DO $$
BEGIN
    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM "Users" WHERE "Username" = 'e2e_test_user') THEN
        INSERT INTO "Users" (
            "Username",
            "Email",
            "PasswordHash",
            "FullName",
            "IsEmailConfirmed",
            "EmailConfirmationToken",
            "CreatedAt",
            "IsActive",
            "SmkVersion",
            "Year"
        ) VALUES (
            'e2e_test_user',
            'e2e@test.sledzspecke.pl',
            '$2a$11$rBNDjVGKzqV5oH.PYV5MiuFoMKV5JWGkK5JxlT1kVgVQfPtWyQOWu', -- Test123!
            'E2E Test User',
            true,
            NULL,
            NOW(),
            true,
            'new',
            1
        );
    END IF;
END $$;