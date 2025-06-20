const { chromium } = require('playwright');
const fs = require('fs');

async function testApp() {
    const browser = await chromium.launch({ 
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });
    const context = await browser.newContext({
        viewport: { width: 1920, height: 1080 }
    });
    const page = await context.newPage();
    
    // Enable console logging
    page.on('console', msg => console.log('BROWSER CONSOLE:', msg.text()));
    page.on('pageerror', err => console.log('PAGE ERROR:', err.message));
    page.on('requestfailed', request => console.log('REQUEST FAILED:', request.url(), request.failure()));

    try {
        console.log('\n=== TESTING LOGIN PAGE ===');
        
        // Go to login page
        await page.goto('https://sledzspecke.pl', { waitUntil: 'networkidle' });
        await page.screenshot({ path: '/tmp/1-login-page.png' });
        console.log('✓ Login page loaded');
        
        // Check if API is reachable from frontend
        const apiHealthResponse = await page.evaluate(async () => {
            try {
                const response = await fetch('https://api.sledzspecke.pl/api/health');
                return { ok: response.ok, status: response.status, data: await response.json() };
            } catch (err) {
                return { error: err.message };
            }
        });
        console.log('API Health Check:', apiHealthResponse);
        
        // Try to login with test user
        console.log('\n=== ATTEMPTING LOGIN ===');
        await page.fill('input[type="email"]', 'test@example.com');
        await page.fill('input[type="password"]', 'Test123!');
        await page.screenshot({ path: '/tmp/2-login-filled.png' });
        
        // Click login button
        await page.click('button:has-text("Zaloguj się")');
        
        // Wait for navigation or error
        await page.waitForTimeout(3000);
        await page.screenshot({ path: '/tmp/3-after-login.png' });
        
        // Check current URL
        const currentUrl = page.url();
        console.log('Current URL after login:', currentUrl);
        
        // Check localStorage for auth tokens
        const localStorage = await page.evaluate(() => {
            const items = {};
            for (let i = 0; i < window.localStorage.length; i++) {
                const key = window.localStorage.key(i);
                items[key] = window.localStorage.getItem(key);
            }
            return items;
        });
        console.log('LocalStorage:', localStorage);
        
        // Check for any error messages
        const errorMessages = await page.$$eval('.error, .alert, [role="alert"]', elements => 
            elements.map(el => el.textContent.trim())
        );
        if (errorMessages.length > 0) {
            console.log('Error messages found:', errorMessages);
        }
        
        // If logged in, check dashboard
        if (currentUrl.includes('dashboard') || currentUrl.includes('home')) {
            console.log('\n=== TESTING DASHBOARD ===');
            await page.screenshot({ path: '/tmp/4-dashboard.png' });
            
            // Look for procedure-related elements
            const procedureElements = await page.$$eval('*[class*="procedure"], *[id*="procedure"], button:has-text("Procedury")', 
                els => els.map(el => ({ tag: el.tagName, text: el.textContent.trim(), class: el.className }))
            );
            console.log('Procedure elements found:', procedureElements);
            
            // Try to navigate to procedures
            const procedureButton = await page.$('button:has-text("Procedury"), a:has-text("Procedury")');
            if (procedureButton) {
                await procedureButton.click();
                await page.waitForTimeout(2000);
                await page.screenshot({ path: '/tmp/5-procedures.png' });
                console.log('✓ Navigated to procedures');
            }
        }
        
        // Save page HTML for debugging
        const pageContent = await page.content();
        fs.writeFileSync('/tmp/page-content.html', pageContent);
        console.log('Page HTML saved to /tmp/page-content.html');
        
    } catch (error) {
        console.error('ERROR:', error);
        await page.screenshot({ path: '/tmp/error-screenshot.png' });
    } finally {
        await browser.close();
    }
}

testApp().catch(console.error);