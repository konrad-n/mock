const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  // Set viewport to match mobile device
  await page.setViewportSize({ width: 375, height: 812 });
  
  try {
    // Mock authentication by setting localStorage
    await page.goto('http://localhost:3001');
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
    
    // Dashboard
    await page.goto('http://localhost:3001/dashboard');
    await page.waitForTimeout(3000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/1-dashboard-maui.png' });
    console.log('Dashboard screenshot saved');
    
    // Dashboard with drawer
    await page.click('button:has-text("☰")');
    await page.waitForTimeout(1000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/2-dashboard-drawer-maui.png' });
    console.log('Dashboard with drawer screenshot saved');
    
    // Close drawer
    await page.click('text=Dashboard');
    await page.waitForTimeout(1000);
    
    // Medical Shifts
    await page.goto('http://localhost:3001/medical-shifts');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/3-medical-shifts-maui.png' });
    console.log('Medical shifts screenshot saved');
    
    // Internships
    await page.goto('http://localhost:3001/internships');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/4-internships-maui.png' });
    console.log('Internships screenshot saved');
    
    // Click on internship details
    await page.click('button:has-text("SZCZEGÓŁY")');
    await page.waitForTimeout(1000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/5-internships-expanded-maui.png' });
    console.log('Internships expanded screenshot saved');
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();