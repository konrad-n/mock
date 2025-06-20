const { chromium } = require('playwright');
const fs = require('fs');

async function testLogin() {
    const browser = await chromium.launch({ 
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });
    const context = await browser.newContext({
        viewport: { width: 1920, height: 1080 }
    });
    const page = await context.newPage();
    
    // Enable console logging
    page.on('console', msg => console.log('BROWSER:', msg.text()));
    page.on('pageerror', err => console.log('PAGE ERROR:', err.message));
    
    // Monitor all network requests
    page.on('request', request => {
        if (request.url().includes('/api/')) {
            console.log('API REQUEST:', request.method(), request.url());
        }
    });
    
    page.on('response', response => {
        if (response.url().includes('/api/')) {
            console.log('API RESPONSE:', response.status(), response.url());
        }
    });

    try {
        console.log('\n=== TESTING LOGIN FLOW ===');
        
        // Go to login page
        await page.goto('https://sledzspecke.pl', { waitUntil: 'networkidle' });
        console.log('✓ Login page loaded');
        
        // Fill login form
        await page.fill('input[type="email"]', 'test@example.com');
        await page.fill('input[type="password"]', 'Test123!');
        await page.screenshot({ path: '/tmp/login-1-filled.png' });
        console.log('✓ Login form filled');
        
        // Click login button and wait for response
        const [response] = await Promise.all([
            page.waitForResponse(resp => resp.url().includes('/api/auth/sign-in')),
            page.click('button:has-text("Zaloguj się")')
        ]);
        
        console.log('Login API response:', response.status());
        if (response.ok()) {
            const responseData = await response.json();
            console.log('Login successful, token received:', responseData.AccessToken ? 'Yes' : 'No');
        } else {
            console.log('Login failed:', await response.text());
        }
        
        // Wait for navigation
        await page.waitForTimeout(2000);
        
        const currentUrl = page.url();
        console.log('Current URL:', currentUrl);
        await page.screenshot({ path: '/tmp/login-2-after.png' });
        
        // Check localStorage
        const localStorage = await page.evaluate(() => {
            const items = {};
            for (let i = 0; i < window.localStorage.length; i++) {
                const key = window.localStorage.key(i);
                const value = window.localStorage.getItem(key);
                items[key] = value ? value.substring(0, 50) + '...' : null;
            }
            return items;
        });
        console.log('LocalStorage keys:', Object.keys(localStorage));
        
        // If on dashboard, test procedures
        if (!currentUrl.includes('login')) {
            console.log('\n=== TESTING DASHBOARD ===');
            await page.screenshot({ path: '/tmp/dashboard-1.png' });
            
            // Look for navigation elements
            const navItems = await page.$$eval('nav a, nav button, aside a, aside button, [role="navigation"] a', 
                els => els.map(el => ({ 
                    text: el.textContent.trim(), 
                    href: el.href || 'button'
                }))
            );
            console.log('Navigation items:', navItems);
            
            // Try to find procedures link
            const procedureLink = await page.$('a:has-text("Procedur"), button:has-text("Procedur")');
            if (procedureLink) {
                console.log('✓ Found procedures link');
                await procedureLink.click();
                await page.waitForTimeout(2000);
                await page.screenshot({ path: '/tmp/procedures-1.png' });
                
                // Check for module tabs
                const moduleTabs = await page.$$eval('[role="tab"], button[class*="tab"], button[class*="module"]',
                    els => els.map(el => el.textContent.trim())
                );
                console.log('Module tabs:', moduleTabs);
            }
        }
        
    } catch (error) {
        console.error('ERROR:', error);
        await page.screenshot({ path: '/tmp/error-login.png' });
    } finally {
        await browser.close();
    }
}

testLogin().catch(console.error);