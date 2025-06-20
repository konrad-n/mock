const https = require('https');
const fs = require('fs');

async function testSMKExportAPI() {
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
                    
                    // Get user specializations first
                    const userOptions = {
                        hostname: 'api.sledzspecke.pl',
                        port: 443,
                        path: '/api/users/1/specializations',
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${token}`,
                            'Content-Type': 'application/json'
                        }
                    };
                    
                    const userReq = https.request(userOptions, (userRes) => {
                        let userData = '';
                        userRes.on('data', (chunk) => { userData += chunk; });
                        userRes.on('end', () => {
                            console.log('\nUser specializations response:', userRes.statusCode);
                            if (userRes.statusCode === 200) {
                                const specializations = JSON.parse(userData);
                                console.log('Specializations:', specializations);
                                
                                if (specializations.length > 0) {
                                    const specializationId = specializations[0].id;
                                    console.log(`\nTesting export for specialization ID: ${specializationId}`);
                                    
                                    // Test export endpoints
                                    const endpoints = [
                                        `/api/smk/export/${specializationId}/preview`,
                                        `/api/smk/export/${specializationId}/xlsx`,
                                        `/api/export/specialization/${specializationId}/xlsx`,
                                        `/api/smk/validate/${specializationId}`
                                    ];
                                    
                                    endpoints.forEach(endpoint => {
                                        const options = {
                                            hostname: 'api.sledzspecke.pl',
                                            port: 443,
                                            path: endpoint,
                                            method: 'GET',
                                            headers: {
                                                'Authorization': `Bearer ${token}`,
                                                'Accept': 'application/json, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                                            }
                                        };
                                        
                                        const req = https.request(options, (res) => {
                                            let responseData = '';
                                            const chunks = [];
                                            
                                            res.on('data', (chunk) => { 
                                                chunks.push(chunk);
                                                responseData += chunk; 
                                            });
                                            
                                            res.on('end', () => {
                                                console.log(`\n${endpoint}:`);
                                                console.log(`Status: ${res.statusCode}`);
                                                console.log(`Content-Type: ${res.headers['content-type']}`);
                                                
                                                if (res.statusCode === 200) {
                                                    if (endpoint.includes('xlsx')) {
                                                        const buffer = Buffer.concat(chunks);
                                                        const filename = `/tmp/smk-export-${Date.now()}.xlsx`;
                                                        fs.writeFileSync(filename, buffer);
                                                        console.log(`Excel file saved to: ${filename} (${buffer.length} bytes)`);
                                                    } else {
                                                        console.log(`Response:`, responseData.substring(0, 200));
                                                    }
                                                } else {
                                                    console.log(`Error response: ${responseData.substring(0, 200)}`);
                                                }
                                            });
                                        });
                                        
                                        req.on('error', (e) => {
                                            console.error(`Error for ${endpoint}: ${e.message}`);
                                        });
                                        req.end();
                                    });
                                }
                            }
                        });
                    });
                    userReq.end();
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

testSMKExportAPI();