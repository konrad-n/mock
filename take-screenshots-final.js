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
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-1-dashboard.png' });
    console.log('Dashboard screenshot saved');
    
    // Medical Shifts
    await page.goto('http://localhost:3001/medical-shifts');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-2-medical-shifts.png' });
    console.log('Medical shifts screenshot saved');
    
    // Procedures
    await page.goto('http://localhost:3001/procedures');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-3-procedures.png' });
    console.log('Procedures screenshot saved');
    
    // Click expand on first procedure
    await page.click('.maui-card:first-child');
    await page.waitForTimeout(1000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-4-procedures-expanded.png' });
    console.log('Procedures expanded screenshot saved');
    
    // Internships
    await page.goto('http://localhost:3001/internships');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-5-internships.png' });
    console.log('Internships screenshot saved');
    
    // Courses
    await page.goto('http://localhost:3001/courses');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: '/home/ubuntu/projects/mock/screenshots/final-6-courses.png' });
    console.log('Courses screenshot saved');
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();