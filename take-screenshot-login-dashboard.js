const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  // Set viewport to match mobile device
  await page.setViewportSize({ width: 375, height: 812 });
  
  try {
    // Navigate to login page
    await page.goto('http://localhost:3001/login');
    await page.waitForTimeout(2000);
    
    // Take screenshot of login page
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/login-page-maui.png' });
    console.log('Login page screenshot saved');
    
    // Mock authentication by setting localStorage
    await page.evaluate(() => {
      localStorage.setItem('auth-storage', JSON.stringify({
        state: {
          token: 'mock-token',
          user: {
            id: 1,
            email: 'test@example.com',
            firstName: 'Test',
            lastName: 'User'
          },
          isAuthenticated: true
        }
      }));
    });
    
    // Navigate to dashboard
    await page.goto('http://localhost:3001/dashboard');
    await page.waitForTimeout(3000);
    
    // Take screenshot of dashboard
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/dashboard-page-maui.png' });
    console.log('Dashboard page screenshot saved');
    
    // Open drawer
    await page.click('button:has-text("☰")');
    await page.waitForTimeout(1000);
    
    // Take screenshot with drawer open
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/dashboard-drawer-open.png' });
    console.log('Dashboard with drawer screenshot saved');
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();