const { chromium } = require('playwright');
const fs = require('fs');

async function testProcedures() {
    const browser = await chromium.launch({ 
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });
    const context = await browser.newContext({
        viewport: { width: 1920, height: 1080 }
    });
    const page = await context.newPage();
    
    // Enable logging
    page.on('console', msg => console.log('BROWSER:', msg.text()));
    page.on('pageerror', err => console.log('PAGE ERROR:', err.message));
    
    // Monitor API calls
    page.on('response', async response => {
        if (response.url().includes('/api/') && response.url().includes('procedure')) {
            console.log(`API: ${response.status()} ${response.url()}`);
            if (!response.ok() && response.status() !== 304) {
                console.log('Response:', await response.text());
            }
        }
    });

    try {
        console.log('\n=== LOGGING IN ===');
        await page.goto('https://sledzspecke.pl', { waitUntil: 'networkidle' });
        await page.fill('input[type="email"]', 'test@example.com');
        await page.fill('input[type="password"]', 'Test123!');
        await page.click('button:has-text("Zaloguj się")');
        await page.waitForURL('**/dashboard');
        console.log('✓ Logged in successfully');
        
        console.log('\n=== NAVIGATING TO PROCEDURES ===');
        // Click on Procedures in the navigation
        await page.click('text=Procedury');
        await page.waitForTimeout(2000);
        
        const currentUrl = page.url();
        console.log('Current URL:', currentUrl);
        await page.screenshot({ path: '/tmp/procedures-page.png' });
        
        // Check for module tabs
        const moduleTabs = await page.$$eval('[role="tab"], button[class*="tab"], .tab-button, [class*="module-tab"]',
            els => els.map(el => ({ 
                text: el.textContent.trim(),
                classes: el.className,
                active: el.classList.contains('active') || el.getAttribute('aria-selected') === 'true'
            }))
        );
        console.log('Module tabs found:', moduleTabs);
        
        // Look for procedures list
        const procedureElements = await page.$$eval(
            '.procedure-item, [class*="procedure"], tr[class*="procedure"], div[data-procedure]',
            els => els.slice(0, 5).map(el => ({
                text: el.textContent.trim().substring(0, 100),
                tag: el.tagName
            }))
        );
        console.log('Procedure elements:', procedureElements);
        
        // Look for "Add Procedure" button
        const addButtons = await page.$$eval(
            'button:has-text("Dodaj"), button:has-text("DODAJ"), button[class*="add"], a:has-text("Dodaj")',
            els => els.map(el => el.textContent.trim())
        );
        console.log('Add buttons found:', addButtons);
        
        // Check page content structure
        const pageStructure = await page.evaluate(() => {
            const main = document.querySelector('main, [role="main"], .main-content');
            if (!main) return 'No main content found';
            
            return {
                headers: Array.from(main.querySelectorAll('h1, h2, h3')).map(h => h.textContent.trim()),
                tables: main.querySelectorAll('table').length,
                lists: main.querySelectorAll('ul, ol').length,
                forms: main.querySelectorAll('form').length,
                buttons: Array.from(main.querySelectorAll('button')).map(b => b.textContent.trim())
            };
        });
        console.log('Page structure:', pageStructure);
        
        // Try clicking on specialist module tab
        console.log('\n=== SWITCHING TO SPECIALIST MODULE ===');
        const specialistTab = await page.$('text=Moduł Specjalistyczny');
        if (specialistTab) {
            await specialistTab.click();
            await page.waitForTimeout(2000);
            await page.screenshot({ path: '/tmp/procedures-specialist.png' });
            console.log('✓ Switched to specialist module');
        }
        
        // Get the HTML of the procedures section for debugging
        const proceduresHTML = await page.evaluate(() => {
            const content = document.querySelector('main, .content, [role="main"]');
            return content ? content.innerHTML.substring(0, 1000) : 'No content found';
        });
        fs.writeFileSync('/tmp/procedures-content.html', proceduresHTML);
        console.log('Procedures HTML saved to /tmp/procedures-content.html');
        
    } catch (error) {
        console.error('ERROR:', error);
        await page.screenshot({ path: '/tmp/error-procedures.png' });
    } finally {
        await browser.close();
    }
}

testProcedures().catch(console.error);