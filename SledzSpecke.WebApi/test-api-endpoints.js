const { chromium } = require('playwright');

async function testAPIEndpoints() {
    const browser = await chromium.launch({ 
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });
    const context = await browser.newContext({
        viewport: { width: 1920, height: 1080 }
    });
    const page = await context.newPage();
    
    const apiCalls = [];
    
    // Intercept all API calls
    page.on('request', request => {
        const url = request.url();
        if (url.includes('/api/')) {
            console.log(`REQUEST: ${request.method()} ${url}`);
            apiCalls.push({
                method: request.method(),
                url: url,
                headers: request.headers()
            });
        }
    });
    
    page.on('response', async response => {
        const url = response.url();
        if (url.includes('/api/')) {
            const status = response.status();
            console.log(`RESPONSE: ${status} ${url}`);
            if (status >= 400) {
                try {
                    const body = await response.text();
                    console.log(`ERROR BODY: ${body.substring(0, 200)}`);
                } catch (e) {
                    console.log('Could not read error body');
                }
            }
        }
    });

    try {
        // Login
        console.log('\n=== LOGIN ===');
        await page.goto('https://sledzspecke.pl');
        await page.fill('input[type="email"]', 'test@example.com');
        await page.fill('input[type="password"]', 'Test123!');
        await page.click('button:has-text("Zaloguj się")');
        await page.waitForURL('**/dashboard');
        
        // Navigate to procedures
        console.log('\n=== PROCEDURES PAGE ===');
        await page.click('text=Procedury');
        await page.waitForTimeout(3000);
        
        // Try to interact with procedures
        console.log('\n=== CHECKING USER DATA ===');
        const authToken = await page.evaluate(() => localStorage.getItem('authToken'));
        console.log('Auth token exists:', !!authToken);
        
        // Get user info from API
        console.log('\n=== TESTING API DIRECTLY ===');
        const userResponse = await page.evaluate(async (token) => {
            const response = await fetch('https://api.sledzspecke.pl/api/users/me', {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            return {
                status: response.status,
                ok: response.ok,
                data: response.ok ? await response.json() : await response.text()
            };
        }, authToken);
        console.log('User API response:', userResponse);
        
        // Test procedures endpoints
        const procedureEndpoints = [
            '/api/procedures',
            '/api/procedures/user', 
            '/api/modules/1/procedures',
            '/api/procedures/modules/1'
        ];
        
        for (const endpoint of procedureEndpoints) {
            const response = await page.evaluate(async (url, token) => {
                const response = await fetch(`https://api.sledzspecke.pl${url}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });
                return {
                    url: url,
                    status: response.status,
                    ok: response.ok
                };
            }, endpoint, authToken);
            console.log(`Test ${endpoint}:`, response.status);
        }
        
    } catch (error) {
        console.error('ERROR:', error);
    } finally {
        console.log('\n=== SUMMARY OF API CALLS ===');
        apiCalls.forEach(call => {
            console.log(`${call.method} ${call.url}`);
        });
        await browser.close();
    }
}

testAPIEndpoints().catch(console.error);