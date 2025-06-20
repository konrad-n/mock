const puppeteer = require('puppeteer');

(async () => {
    console.log('=== TESTING MODULE SWITCHING ===');
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

        // Test on Dashboard
        console.log('\n2. Testing module switching on Dashboard...');
        await page.goto('https://sledzspecke.pl/dashboard');
        await new Promise(resolve => setTimeout(resolve, 2000));

        // Look for module selector
        const moduleSelector = await page.$('[role="button"][aria-haspopup="listbox"]');
        if (moduleSelector) {
            console.log('✓ Module selector found on dashboard');
            
            // Click to open dropdown
            await moduleSelector.click();
            await new Promise(resolve => setTimeout(resolve, 500));
            
            // Get available modules
            const modules = await page.evaluate(() => {
                const items = document.querySelectorAll('[role="option"]');
                return Array.from(items).map(item => item.innerText);
            });
            console.log('Available modules:', modules);
            
            // Take screenshot of dropdown
            await page.screenshot({ path: '/tmp/module-dropdown.png' });
        } else {
            console.log('✗ Module selector not found on dashboard');
        }

        // Test on Procedures page
        console.log('\n3. Testing module switching on Procedures page...');
        await page.goto('https://sledzspecke.pl/procedures');
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Look for tabs
        const tabs = await page.evaluate(() => {
            const tabElements = document.querySelectorAll('[role="tab"]');
            return Array.from(tabElements).map(tab => ({
                text: tab.innerText,
                selected: tab.getAttribute('aria-selected') === 'true'
            }));
        });
        
        if (tabs.length > 0) {
            console.log('✓ Module tabs found:', tabs);
            
            // Click on specialist tab if available
            const specialistTab = await page.$('[role="tab"]:nth-child(2)');
            if (specialistTab) {
                await specialistTab.click();
                await new Promise(resolve => setTimeout(resolve, 1000));
                console.log('✓ Clicked on Specialist tab');
                
                // Take screenshot
                await page.screenshot({ path: '/tmp/specialist-module.png' });
                
                // Get updated content
                const content = await page.evaluate(() => {
                    const summaryElement = document.querySelector('[class*="summary"]');
                    return summaryElement ? summaryElement.innerText : 'No summary found';
                });
                console.log('Specialist module content:', content);
            }
        } else {
            console.log('✗ No module tabs found on procedures page');
        }

        // Test on Medical Shifts page
        console.log('\n4. Testing module switching on Medical Shifts page...');
        await page.goto('https://sledzspecke.pl/medical-shifts');
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        const shiftsModuleSelector = await page.$('[role="button"][aria-haspopup="listbox"]');
        if (shiftsModuleSelector) {
            console.log('✓ Module selector found on medical shifts page');
            await page.screenshot({ path: '/tmp/shifts-module-selector.png' });
        } else {
            console.log('✗ Module selector not found on medical shifts page');
        }

        console.log('\n=== MODULE SWITCHING TEST COMPLETE ===');

    } catch (error) {
        console.error('Error:', error);
        await page.screenshot({ path: '/tmp/module-test-error.png' });
    } finally {
        await browser.close();
    }
})();