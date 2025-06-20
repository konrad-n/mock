const https = require('https');

async function createTestUser() {
    const userData = {
        email: "manualtest@example.com",
        password: "Test123!",
        firstName: "Manual",
        lastName: "Test",
        pesel: "92010112345",
        pwzNumber: "1234567",
        phoneNumber: "+48123456789",
        dateOfBirth: "1992-01-01T00:00:00Z",
        correspondenceAddress: {
            street: "ul. Testowa",
            houseNumber: "1",
            apartmentNumber: null,
            postalCode: "00-001",
            city: "Warsaw",
            province: "Mazowieckie"
        }
    };

    const options = {
        hostname: 'api.sledzspecke.pl',
        port: 443,
        path: '/api/auth/sign-up',
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Content-Length': Buffer.byteLength(JSON.stringify(userData))
        }
    };

    return new Promise((resolve, reject) => {
        const req = https.request(options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log('Status:', res.statusCode);
                console.log('Response:', data);
                if (res.statusCode === 200 || res.statusCode === 201) {
                    resolve(JSON.parse(data));
                } else {
                    reject(new Error(`Status ${res.statusCode}: ${data}`));
                }
            });
        });
        
        req.on('error', (e) => {
            reject(e);
        });
        
        req.write(JSON.stringify(userData));
        req.end();
    });
}

createTestUser()
    .then(() => console.log('User created successfully'))
    .catch(err => console.error('Error:', err.message));