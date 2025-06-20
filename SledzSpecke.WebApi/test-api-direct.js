const https = require('https');

async function testProceduresAPI() {
    // First, get a token
    const loginData = JSON.stringify({
        email: "test@example.com",
        password: "Test123!"
    });

    const loginOptions = {
        hostname: 'api.sledzspecke.pl',
        port: 443,
        path: '/api/auth/sign-in',
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Content-Length': loginData.length
        }
    };

    return new Promise((resolve) => {
        const loginReq = https.request(loginOptions, (res) => {
            let data = '';
            res.on('data', (chunk) => { data += chunk; });
            res.on('end', () => {
                console.log('Login response:', res.statusCode);
                if (res.statusCode === 200) {
                    const auth = JSON.parse(data);
                    const token = auth.AccessToken;
                    console.log('Token received');
                    
                    // Test various endpoints
                    const endpoints = [
                        '/api/procedures?internshipId=1',
                        '/api/Procedures?internshipId=1',
                        '/api/procedures',
                        '/api/procedures/user',
                        '/api/users/me',
                        '/api/users/1/specializations'
                    ];
                    
                    endpoints.forEach(endpoint => {
                        const options = {
                            hostname: 'api.sledzspecke.pl',
                            port: 443,
                            path: endpoint,
                            method: 'GET',
                            headers: {
                                'Authorization': `Bearer ${token}`,
                                'Content-Type': 'application/json'
                            }
                        };
                        
                        const req = https.request(options, (res) => {
                            let responseData = '';
                            res.on('data', (chunk) => { responseData += chunk; });
                            res.on('end', () => {
                                console.log(`\n${endpoint}:`);
                                console.log(`Status: ${res.statusCode}`);
                                if (res.statusCode !== 200) {
                                    console.log(`Response: ${responseData.substring(0, 200)}`);
                                } else {
                                    console.log(`Success - Data length: ${responseData.length} bytes`);
                                }
                            });
                        });
                        req.on('error', (e) => {
                            console.error(`Error for ${endpoint}: ${e.message}`);
                        });
                        req.end();
                    });
                } else {
                    console.log('Login failed:', data);
                }
                resolve();
            });
        });
        
        loginReq.on('error', (e) => {
            console.error('Login error:', e);
            resolve();
        });
        
        loginReq.write(loginData);
        loginReq.end();
    });
}

testProceduresAPI();