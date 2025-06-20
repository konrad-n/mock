const puppeteer = require('puppeteer');

(async () => {
    console.log('=== ANALYZING PAGE STRUCTURE ===');
    const browser = await puppeteer.launch({
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });

    const page = await browser.newPage();
    await page.setViewport({ width: 1920, height: 1080 });

    try {
        // Login
        console.log('\n1. Logging in...');
        await page.goto('https://sledzspecke.pl/login');
        await page.waitForSelector('input[type="email"]', { timeout: 10000 });

        await page.type('input[type="email"]', 'test@example.com');
        await page.type('input[type="password"]', 'Test123!');
        
        await page.click('button[type="submit"]');
        await page.waitForNavigation({ waitUntil: 'networkidle2' });
        console.log('✓ Logged in successfully');

        // Analyze Dashboard
        console.log('\n2. Analyzing Dashboard structure...');
        await page.goto('https://sledzspecke.pl/dashboard');
        await new Promise(resolve => setTimeout(resolve, 3000));
        
        const dashboardStructure = await page.evaluate(() => {
            const findSelectors = () => {
                const selectors = {
                    dropdowns: document.querySelectorAll('select, [role="button"][aria-haspopup="listbox"]').length,
                    buttons: document.querySelectorAll('button').length,
                    headers: Array.from(document.querySelectorAll('h1, h2, h3, h4, h5, h6')).map(h => h.innerText),
                    navigation: Array.from(document.querySelectorAll('nav a, [role="navigation"] a')).map(a => a.innerText),
                    cards: document.querySelectorAll('[class*="Card"]').length,
                    topBarText: document.querySelector('[class*="AppBar"]')?.innerText || 'No AppBar found'
                };
                return selectors;
            };
            return findSelectors();
        });
        
        console.log('Dashboard structure:', JSON.stringify(dashboardStructure, null, 2));
        await page.screenshot({ path: '/tmp/dashboard-structure.png' });

        // Analyze Procedures page in detail
        console.log('\n3. Analyzing Procedures page structure...');
        await page.goto('https://sledzspecke.pl/procedures');
        await new Promise(resolve => setTimeout(resolve, 3000));
        
        const proceduresStructure = await page.evaluate(() => {
            const findElements = () => {
                // Look for any tab-like elements
                const tabs = document.querySelectorAll('[role="tab"], .MuiTab-root, button[class*="Tab"]');
                const toggles = document.querySelectorAll('[class*="Toggle"], [class*="Switch"]');
                const chips = document.querySelectorAll('[class*="Chip"]');
                
                return {
                    tabs: Array.from(tabs).map(t => ({ text: t.innerText, classes: t.className })),
                    toggles: Array.from(toggles).map(t => ({ text: t.innerText, classes: t.className })),
                    chips: Array.from(chips).map(c => ({ text: c.innerText, classes: c.className })),
                    pageTitle: document.querySelector('h1, h2, h3, h4')?.innerText || 'No title',
                    hasContent: document.body.innerText.includes('Podstawowy') || document.body.innerText.includes('Specjalistyczny')
                };
            };
            return findElements();
        });
        
        console.log('Procedures structure:', JSON.stringify(proceduresStructure, null, 2));
        await page.screenshot({ path: '/tmp/procedures-structure.png' });

        // Look specifically for module switching elements
        console.log('\n4. Looking for module switching elements...');
        const moduleElements = await page.evaluate(() => {
            // Search for text containing module names
            const allText = document.body.innerText;
            const hasBasicModule = allText.includes('Moduł podstawowy');
            const hasSpecialistModule = allText.includes('Moduł specjalistyczny');
            const hasOldSMK = allText.includes('Stary SMK');
            const hasNewSMK = allText.includes('Nowy SMK');
            
            // Look for any clickable elements with module text
            const clickableWithModule = Array.from(document.querySelectorAll('button, [role="button"], a')).filter(el => 
                el.innerText.includes('Podstawowy') || el.innerText.includes('Specjalistyczny')
            ).map(el => ({ tag: el.tagName, text: el.innerText, classes: el.className }));
            
            return {
                hasBasicModule,
                hasSpecialistModule,
                hasOldSMK,
                hasNewSMK,
                clickableWithModule
            };
        });
        
        console.log('Module elements:', JSON.stringify(moduleElements, null, 2));

    } catch (error) {
        console.error('Error:', error);
    } finally {
        await browser.close();
    }
})();