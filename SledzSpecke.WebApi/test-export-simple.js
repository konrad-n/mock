const https = require('https');
const fs = require('fs');

async function testExport() {
    // Get token
    const loginData = JSON.stringify({ email: "test@example.com", password: "Test123!" });
    
    const token = await new Promise((resolve) => {
        const req = https.request({
            hostname: 'api.sledzspecke.pl',
            port: 443,
            path: '/api/auth/sign-in',
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Content-Length': loginData.length
            }
        }, (res) => {
            let data = '';
            res.on('data', (chunk) => { data += chunk; });
            res.on('end', () => {
                if (res.statusCode === 200) {
                    const auth = JSON.parse(data);
                    resolve(auth.AccessToken);
                } else {
                    console.error('Login failed');
                    resolve(null);
                }
            });
        });
        req.write(loginData);
        req.end();
    });

    if (!token) return;
    console.log('✓ Logged in successfully');

    // Test export endpoint for specialization ID 1
    console.log('\nTesting SMK export endpoints for specialization ID 1...\n');
    
    const testEndpoint = async (path) => {
        return new Promise((resolve) => {
            const chunks = [];
            const req = https.request({
                hostname: 'api.sledzspecke.pl',
                port: 443,
                path: path,
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Accept': '*/*'
                }
            }, (res) => {
                res.on('data', (chunk) => chunks.push(chunk));
                res.on('end', () => {
                    const buffer = Buffer.concat(chunks);
                    console.log(`${path}:`);
                    console.log(`  Status: ${res.statusCode}`);
                    console.log(`  Content-Type: ${res.headers['content-type']}`);
                    console.log(`  Content-Length: ${res.headers['content-length'] || buffer.length}`);
                    
                    if (res.statusCode === 200 && res.headers['content-type']?.includes('spreadsheetml')) {
                        const filename = `/tmp/export-${Date.now()}.xlsx`;
                        fs.writeFileSync(filename, buffer);
                        console.log(`  ✓ Excel file saved to: ${filename}`);
                    } else if (res.statusCode === 200) {
                        console.log(`  Response preview: ${buffer.toString().substring(0, 100)}...`);
                    } else {
                        console.log(`  Error: ${buffer.toString().substring(0, 200)}`);
                    }
                    console.log();
                    resolve();
                });
            });
            req.on('error', (e) => {
                console.error(`${path}: Request error - ${e.message}`);
                resolve();
            });
            req.end();
        });
    };

    // Test all export endpoints
    await testEndpoint('/api/smk/export/1/preview');
    await testEndpoint('/api/smk/export/1/xlsx');
    await testEndpoint('/api/export/specialization/1/xlsx');
    await testEndpoint('/api/smk/validate/1');
    
    console.log('✓ Export test complete');
}

testExport();