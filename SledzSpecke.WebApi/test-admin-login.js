const { chromium } = require('playwright');

(async () => {
  console.log('Testing admin login...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext();
  const page = await context.newPage();

  try {
    // Go to login page
    console.log('Navigating to https://sledzspecke.pl...');
    await page.goto('https://sledzspecke.pl');
    
    // Login with admin credentials
    console.log('Filling login form with admin@sledzspecke.pl...');
    await page.fill('[name="email"]', 'admin@sledzspecke.pl');
    await page.fill('[name="password"]', 'Test123');
    
    console.log('Clicking login button...');
    await page.click('button[type="submit"]');
    
    // Wait for navigation
    await page.waitForTimeout(2000);
    
    // Get auth token from localStorage
    const token = await page.evaluate(() => localStorage.getItem('auth_token'));
    if (token) {
      console.log('✅ Admin login successful! Token received');
      
      // Decode JWT to check role
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('User role:', payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
      console.log('User ID:', payload.sub);
    } else {
      console.log('❌ Login failed - no token');
    }
    
    // Test API access with admin token
    console.log('\nTesting admin API access...');
    const response = await page.evaluate(async (token) => {
      const res = await fetch('https://api.sledzspecke.pl/api/admin/trigger-data-migration', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      return {
        status: res.status,
        statusText: res.statusText,
        body: await res.text()
      };
    }, token);
    
    console.log('Admin API response:', response.status, response.statusText);
    if (response.status === 200) {
      console.log('✅ Admin API access successful!');
    } else {
      console.log('❌ Admin API access failed:', response.body);
    }
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();