const puppeteer = require('puppeteer');

(async () => {
    console.log('=== LAUNCHING BROWSER ===');
    const browser = await puppeteer.launch({
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    });

    const page = await browser.newPage();
    await page.setViewport({ width: 1920, height: 1080 });

    // Listen for console logs
    page.on('console', msg => {
        if (msg.type() === 'error') {
            console.log('BROWSER ERROR:', msg.text());
        }
    });

    page.on('pageerror', error => {
        console.log('PAGE ERROR:', error.message);
    });

    try {
        // Login
        console.log('=== LOGGING IN ===');
        await page.goto('https://sledzspecke.pl/login');
        await page.waitForSelector('input[type="email"]', { timeout: 10000 });

        await page.type('input[type="email"]', 'test@example.com');
        await page.type('input[type="password"]', 'Test123!');
        
        await page.click('button[type="submit"]');
        await page.waitForNavigation({ waitUntil: 'networkidle2' });
        console.log('✓ Logged in successfully');

        // Navigate to procedures
        console.log('\n=== NAVIGATING TO PROCEDURES ===');
        await page.goto('https://sledzspecke.pl/procedures');
        await new Promise(resolve => setTimeout(resolve, 3000)); // Wait for page to load
        
        // Take screenshot
        await page.screenshot({ path: '/tmp/procedures-page-error.png' });
        console.log('✓ Screenshot saved to /tmp/procedures-page-error.png');

        // Get page content
        const content = await page.evaluate(() => {
            return {
                title: document.title,
                url: window.location.href,
                bodyText: document.body.innerText,
                errorElements: Array.from(document.querySelectorAll('.error, .MuiAlert-root')).map(el => el.innerText),
                hasContent: document.querySelector('[class*="MuiContainer"]') !== null
            };
        });

        console.log('\nPage info:', JSON.stringify(content, null, 2));

        // Try to get any error messages from React
        const reactErrors = await page.evaluate(() => {
            const errorBoundary = document.querySelector('[class*="error"]');
            if (errorBoundary) {
                return errorBoundary.innerText;
            }
            return null;
        });

        if (reactErrors) {
            console.log('\nReact error:', reactErrors);
        }

    } catch (error) {
        console.error('Error:', error);
    } finally {
        await browser.close();
    }
})();