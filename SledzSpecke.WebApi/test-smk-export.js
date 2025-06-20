const puppeteer = require('puppeteer');
const fs = require('fs');
const path = require('path');

(async () => {
    console.log('=== TESTING SMK EXPORT FUNCTIONALITY ===');
    const browser = await puppeteer.launch({
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });

    const page = await browser.newPage();
    await page.setViewport({ width: 1920, height: 1080 });

    // Set download behavior
    const downloadPath = '/tmp/smk-exports';
    if (!fs.existsSync(downloadPath)) {
        fs.mkdirSync(downloadPath, { recursive: true });
    }
    
    const client = await page.target().createCDPSession();
    await client.send('Page.setDownloadBehavior', {
        behavior: 'allow',
        downloadPath: downloadPath,
    });

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

        // Navigate to export page
        console.log('\n2. Navigating to Export page...');
        await page.goto('https://sledzspecke.pl/eksport');
        await new Promise(resolve => setTimeout(resolve, 3000));

        // Analyze export page
        const exportPageInfo = await page.evaluate(() => {
            const pageText = document.body.innerText;
            const buttons = Array.from(document.querySelectorAll('button')).map(btn => ({
                text: btn.innerText,
                disabled: btn.disabled,
                classes: btn.className
            }));
            const selects = Array.from(document.querySelectorAll('select')).map(sel => ({
                name: sel.name || sel.id,
                options: Array.from(sel.options).map(opt => opt.text)
            }));
            const inputs = Array.from(document.querySelectorAll('input[type="checkbox"], input[type="radio"]')).map(inp => ({
                type: inp.type,
                checked: inp.checked,
                label: inp.parentElement?.innerText || inp.value
            }));
            
            return {
                hasOldSMK: pageText.includes('Stary SMK') || pageText.includes('old'),
                hasNewSMK: pageText.includes('Nowy SMK') || pageText.includes('new'),
                hasExcelExport: pageText.toLowerCase().includes('excel') || pageText.toLowerCase().includes('xlsx'),
                buttons,
                selects,
                inputs
            };
        });

        console.log('Export page info:', JSON.stringify(exportPageInfo, null, 2));
        await page.screenshot({ path: '/tmp/export-page.png' });

        // Look for export buttons
        console.log('\n3. Looking for export buttons...');
        const exportButtons = await page.evaluate(() => {
            const buttons = Array.from(document.querySelectorAll('button'));
            const exportBtn = buttons.find(btn => 
                btn.innerText.includes('Export') || 
                btn.innerText.includes('Eksport') ||
                btn.innerText.includes('Pobierz') ||
                btn.innerText.includes('Download')
            );
            return exportBtn ? { found: true, text: exportBtn.innerText } : { found: false };
        });
        
        if (exportButtons.found) {
            console.log('✓ Found export button:', exportButtons.text);
            
            // Click export button by finding it again
            await page.evaluate(() => {
                const buttons = Array.from(document.querySelectorAll('button'));
                const exportBtn = buttons.find(btn => 
                    btn.innerText.includes('Export') || 
                    btn.innerText.includes('Eksport') ||
                    btn.innerText.includes('Pobierz') ||
                    btn.innerText.includes('Download')
                );
                if (exportBtn) exportBtn.click();
            });
            console.log('✓ Clicked export button');
            
            // Wait for download or response
            await new Promise(resolve => setTimeout(resolve, 5000));
            
            // Check for downloaded files
            const files = fs.readdirSync(downloadPath);
            if (files.length > 0) {
                console.log('✓ Files downloaded:', files);
                files.forEach(file => {
                    const filePath = path.join(downloadPath, file);
                    const stats = fs.statSync(filePath);
                    console.log(`  - ${file} (${stats.size} bytes)`);
                });
            } else {
                console.log('✗ No files downloaded');
            }
        } else {
            console.log('✗ Export button not found');
            
            // Try to find any export-related elements
            const exportElements = await page.evaluate(() => {
                const elements = Array.from(document.querySelectorAll('*')).filter(el => 
                    el.innerText && (
                        el.innerText.toLowerCase().includes('export') ||
                        el.innerText.toLowerCase().includes('eksport') ||
                        el.innerText.toLowerCase().includes('xlsx') ||
                        el.innerText.toLowerCase().includes('excel')
                    )
                );
                return elements.slice(0, 5).map(el => ({
                    tag: el.tagName,
                    text: el.innerText.substring(0, 100),
                    clickable: el.tagName === 'BUTTON' || el.tagName === 'A'
                }));
            });
            console.log('Export-related elements found:', exportElements);
        }

        // Check API endpoint
        console.log('\n4. Testing SMK export API endpoint...');
        const apiResponse = await page.evaluate(async () => {
            try {
                const response = await fetch('https://api.sledzspecke.pl/api/smk/export', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
                        'Accept': 'application/json'
                    }
                });
                
                return {
                    status: response.status,
                    statusText: response.statusText,
                    headers: Object.fromEntries(response.headers.entries())
                };
            } catch (error) {
                return { error: error.message };
            }
        });
        console.log('API response:', apiResponse);

        // Try alternate endpoints
        const endpoints = [
            '/api/export/smk',
            '/api/export/excel',
            '/api/specializations/export',
            '/api/users/export'
        ];

        for (const endpoint of endpoints) {
            const testResponse = await page.evaluate(async (ep) => {
                try {
                    const response = await fetch(`https://api.sledzspecke.pl${ep}`, {
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
                        }
                    });
                    return { endpoint: ep, status: response.status };
                } catch (error) {
                    return { endpoint: ep, error: error.message };
                }
            }, endpoint);
            console.log('Endpoint test:', testResponse);
        }

        console.log('\n=== SMK EXPORT TEST COMPLETE ===');

    } catch (error) {
        console.error('Error:', error);
        await page.screenshot({ path: '/tmp/export-test-error.png' });
    } finally {
        await browser.close();
    }
})();