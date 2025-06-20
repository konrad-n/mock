-- Create admin user for testing
-- Email: admin@sledzspecke.pl
-- Password: Admin123!

-- First check if user exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Users" WHERE "Email" = 'admin@sledzspecke.pl') THEN
        -- Insert admin user with proper password hash for Admin123!
        INSERT INTO "Users" (
            "Id",
            "Email",
            "Password",
            "FirstName",
            "LastName",
            "PhoneNumber",
            "DateOfBirth",
            "RegistrationDate",
            "CreatedAt",
            "EmailNotificationsEnabled",
            "NotificationsEnabled",
            "CorrespondenceAddress_Street",
            "CorrespondenceAddress_HouseNumber",
            "CorrespondenceAddress_City",
            "CorrespondenceAddress_PostalCode",
            "CorrespondenceAddress_Province",
            "CorrespondenceAddress_Country"
        ) VALUES (
            (SELECT COALESCE(MAX("Id"), 0) + 1 FROM "Users"),
            'admin@sledzspecke.pl',
            'PJCMBBji5Q1GIszD/mPMuQ==.+5QPko0AqtuwA8H956QOsRtPdFh0YQDujFPunocJ0NQ=',  -- Test123
            'Admin',
            'System',
            '+48123456789',
            '1980-01-01'::timestamp with time zone,
            NOW(),
            NOW(),
            true,
            true,
            'Administracyjna',
            '1',
            'Warszawa',
            '00-001',
            'Mazowieckie',
            'Polska'
        );
        
        RAISE NOTICE 'Admin user created successfully';
    ELSE
        RAISE NOTICE 'Admin user already exists';
    END IF;
END
$$;

-- Verify the admin user was created
SELECT u."Id", u."Email", u."FirstName", u."LastName"
FROM "Users" u
WHERE u."Email" = 'admin@sledzspecke.pl';