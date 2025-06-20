const crypto = require('crypto');

function verifyPassword(password, hashedPassword) {
    // Parse the stored hash (format: salt.hash)
    const parts = hashedPassword.split('.');
    if (parts.length !== 2) {
        console.log('Invalid hash format');
        return false;
    }
    
    const salt = Buffer.from(parts[0], 'base64');
    const hash = Buffer.from(parts[1], 'base64');
    
    // Generate hash with same salt
    const testHash = crypto.pbkdf2Sync(password, salt, 100000, 32, 'sha256');
    
    console.log('Salt:', parts[0]);
    console.log('Expected hash:', parts[1]);
    console.log('Generated hash:', testHash.toString('base64'));
    console.log('Match:', testHash.equals(hash));
    
    return testHash.equals(hash);
}

// Test with the hash we created
const storedHash = '2PbrCYP8iJbtmRt1NHP09w==.9FYBD3Hk5TkaN4MxURrc85Im1pLVWeKY+rVfYLNWTEk=';
const password = 'Admin123!';

console.log('Testing password verification...');
const isValid = verifyPassword(password, storedHash);
console.log('Password valid:', isValid);