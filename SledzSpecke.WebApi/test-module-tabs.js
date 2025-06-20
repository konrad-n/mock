const puppeteer = require('puppeteer');

(async () => {
    console.log('=== TESTING MODULE TAB SWITCHING ===');
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

        // Navigate to procedures
        console.log('\n2. Navigating to Procedures page...');
        await page.goto('https://sledzspecke.pl/procedures');
        await new Promise(resolve => setTimeout(resolve, 3000));

        // Get initial state
        console.log('\n3. Getting initial module state...');
        const initialState = await page.evaluate(() => {
            const activeTab = document.querySelector('.maui-module-tab.active');
            const moduleTitle = document.querySelector('h2, h3')?.innerText;
            const procedureCount = document.querySelectorAll('[class*="procedure-item"], [class*="ListItem"]').length;
            
            return {
                activeTab: activeTab?.innerText || 'None',
                moduleTitle: moduleTitle || 'No title',
                procedureCount
            };
        });
        console.log('Initial state:', initialState);

        // Click on Specialist tab
        console.log('\n4. Clicking on Specialist module tab...');
        const specialistTab = await page.$('button.maui-module-tab:not(.active)');
        if (specialistTab) {
            await specialistTab.click();
            await new Promise(resolve => setTimeout(resolve, 2000));
            console.log('✓ Clicked on Specialist tab');

            // Get updated state
            const updatedState = await page.evaluate(() => {
                const activeTab = document.querySelector('.maui-module-tab.active');
                const moduleTitle = document.querySelector('h2, h3')?.innerText;
                const procedureCount = document.querySelectorAll('[class*="procedure-item"], [class*="ListItem"]').length;
                const summaryText = document.querySelector('[class*="summary"]')?.innerText;
                
                return {
                    activeTab: activeTab?.innerText || 'None',
                    moduleTitle: moduleTitle || 'No title',
                    procedureCount,
                    summaryText: summaryText || 'No summary'
                };
            });
            console.log('Updated state:', updatedState);

            // Take screenshot
            await page.screenshot({ path: '/tmp/specialist-module-active.png' });
            console.log('✓ Screenshot saved to /tmp/specialist-module-active.png');

            // Click back to Basic tab
            console.log('\n5. Clicking back to Basic module tab...');
            const basicTab = await page.$('button.maui-module-tab:first-child');
            if (basicTab) {
                await basicTab.click();
                await new Promise(resolve => setTimeout(resolve, 2000));
                console.log('✓ Clicked on Basic tab');

                const finalState = await page.evaluate(() => {
                    const activeTab = document.querySelector('.maui-module-tab.active');
                    return {
                        activeTab: activeTab?.innerText || 'None'
                    };
                });
                console.log('Final state:', finalState);
            }
        } else {
            console.log('✗ Specialist tab not found');
        }

        // Test on other pages
        console.log('\n6. Testing module tabs on Medical Shifts page...');
        await page.goto('https://sledzspecke.pl/medical-shifts');
        await new Promise(resolve => setTimeout(resolve, 2000));

        const shiftsModuleTabs = await page.evaluate(() => {
            const tabs = document.querySelectorAll('.maui-module-tab');
            return Array.from(tabs).map(tab => ({
                text: tab.innerText,
                active: tab.classList.contains('active')
            }));
        });

        if (shiftsModuleTabs.length > 0) {
            console.log('✓ Module tabs found on Medical Shifts:', shiftsModuleTabs);
        } else {
            console.log('✗ No module tabs found on Medical Shifts page');
        }

        console.log('\n=== MODULE TAB SWITCHING TEST COMPLETE ===');

    } catch (error) {
        console.error('Error:', error);
        await page.screenshot({ path: '/tmp/module-test-error.png' });
    } finally {
        await browser.close();
    }
})();