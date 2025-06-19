const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  // Set viewport to match mobile device
  await page.setViewportSize({ width: 375, height: 812 });
  
  try {
    // Navigate directly to dashboard (it will redirect to login)
    await page.goto('http://localhost:3001/dashboard');
    await page.waitForTimeout(3000);
    
    // Take screenshot of whatever page we're on
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/current-page.png' });
    
    console.log('Screenshot saved to screenshots/current-page.png');
    console.log('Current URL:', page.url());
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();