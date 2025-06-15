const { chromium } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

async function captureScreenshots() {
  const browser = await chromium.launch({ 
    headless: true,
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });
  
  const context = await browser.newContext({
    viewport: { width: 1920, height: 1080 },
    locale: 'pl-PL',
    ignoreHTTPSErrors: true
  });
  
  const page = await context.newPage();
  const screenshotsDir = 'Reports/Screenshots';
  
  if (!fs.existsSync(screenshotsDir)) {
    fs.mkdirSync(screenshotsDir, { recursive: true });
  }
  
  const screenshots = [];
  
  try {
    console.log('Capturing homepage...');
    await page.goto('https://sledzspecke.pl', { 
      waitUntil: 'domcontentloaded', 
      timeout: 30000 
    });
    await page.waitForTimeout(2000);
    
    const homepageFile = path.join(screenshotsDir, 'homepage.png');
    await page.screenshot({ path: homepageFile, fullPage: true });
    screenshots.push({ name: 'Homepage', file: 'homepage.png', status: 'success' });
    console.log('✓ Homepage captured');
    
    // Try to find and click login
    try {
      console.log('Looking for login...');
      const loginSelectors = ['text=Zaloguj', 'text=Login', 'a[href*="login"]', 'button:has-text("Zaloguj")'];
      let clicked = false;
      
      for (const selector of loginSelectors) {
        try {
          await page.click(selector, { timeout: 5000 });
          clicked = true;
          break;
        } catch (e) {
          // Continue trying other selectors
        }
      }
      
      if (clicked) {
        await page.waitForTimeout(2000);
        const loginFile = path.join(screenshotsDir, 'login-page.png');
        await page.screenshot({ path: loginFile, fullPage: true });
        screenshots.push({ name: 'Login Page', file: 'login-page.png', status: 'success' });
        console.log('✓ Login page captured');
      }
    } catch (e) {
      console.log('Could not navigate to login page');
    }
    
    // Try API endpoints
    console.log('Capturing API documentation...');
    await page.goto('https://api.sledzspecke.pl/swagger', { 
      waitUntil: 'domcontentloaded', 
      timeout: 30000 
    });
    await page.waitForTimeout(2000);
    
    const swaggerFile = path.join(screenshotsDir, 'api-swagger.png');
    await page.screenshot({ path: swaggerFile, fullPage: true });
    screenshots.push({ name: 'API Documentation', file: 'api-swagger.png', status: 'success' });
    console.log('✓ API documentation captured');
    
  } catch (error) {
    console.error('Error during capture:', error.message);
    
    // Capture error state
    const errorFile = path.join(screenshotsDir, 'error-state.png');
    await page.screenshot({ path: errorFile, fullPage: true });
    screenshots.push({ name: 'Error State', file: 'error-state.png', status: 'error' });
  }
  
  await browser.close();
  
  // Generate results JSON
  const results = {
    timestamp: new Date().toISOString(),
    screenshots: screenshots,
    testSummary: {
      total: 7,
      passed: 0,
      failed: 7,
      skipped: 0
    }
  };
  
  fs.writeFileSync(
    path.join(screenshotsDir, 'results.json'), 
    JSON.stringify(results, null, 2)
  );
  
  console.log('\nScreenshot capture complete!');
  console.log(`Results saved to: ${screenshotsDir}`);
  
  return results;
}

// Run if called directly
if (require.main === module) {
  captureScreenshots().catch(console.error);
}

module.exports = { captureScreenshots };