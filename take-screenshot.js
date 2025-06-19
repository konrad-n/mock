const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  // Set viewport to match mobile device
  await page.setViewportSize({ width: 375, height: 812 });
  
  try {
    // Navigate to login page first
    await page.goto('http://localhost:3001/login');
    await page.waitForTimeout(2000);
    
    // Take screenshot of login page
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/login-page.png' });
    
    // Login with test credentials
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Test123!');
    await page.click('button[type="submit"]');
    
    // Wait for navigation to dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 });
    await page.waitForTimeout(2000);
    
    // Take screenshot of dashboard
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/dashboard-page.png' });
    
    console.log('Screenshots saved successfully!');
  } catch (error) {
    console.error('Error taking screenshots:', error);
  } finally {
    await browser.close();
  }
})();