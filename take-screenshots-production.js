const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  // Set viewport to match mobile device
  await page.setViewportSize({ width: 375, height: 812 });
  
  try {
    // Navigate to production site
    await page.goto('https://sledzspecke.pl');
    await page.waitForTimeout(3000);
    
    // Take screenshot of login page
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/prod-1-login.png' });
    console.log('Production login page screenshot saved');
    
    // Try to login with test credentials
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'Test123!');
    await page.click('button[type="submit"]');
    
    // Wait a bit
    await page.waitForTimeout(5000);
    
    // Take screenshot of whatever page we're on
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/prod-2-after-login.png' });
    console.log('Production after login screenshot saved');
    
    // If we're on dashboard, try to capture more
    if (page.url().includes('dashboard')) {
      // Click hamburger menu if exists
      try {
        await page.click('button:has-text("☰")', { timeout: 2000 });
        await page.waitForTimeout(1000);
        await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/prod-3-drawer.png' });
        console.log('Production drawer screenshot saved');
      } catch (e) {
        console.log('No hamburger menu found');
      }
    }
    
  } catch (error) {
    console.error('Error:', error);
    console.log('Current URL:', page.url());
  } finally {
    await browser.close();
  }
})();