const { chromium } = require('playwright');

(async () => {
  console.log('Testing admin login with cache disabled...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    // Disable cache
    bypassCSP: true,
    ignoreHTTPSErrors: true,
    extraHTTPHeaders: {
      'Cache-Control': 'no-cache, no-store, must-revalidate',
      'Pragma': 'no-cache'
    }
  });
  const page = await context.newPage();

  // Clear any existing cache/storage
  await context.clearCookies();
  
  try {
    // Go to login page with cache busting
    console.log('Navigating to https://sledzspecke.pl with cache disabled...');
    await page.goto('https://sledzspecke.pl?_t=' + Date.now(), { 
      waitUntil: 'networkidle',
      // Force reload
      waitForLoadState: 'domcontentloaded'
    });
    
    // Force reload to ensure we get latest JS
    await page.reload({ waitUntil: 'networkidle' });
    
    // Wait a bit for JS to initialize
    await page.waitForTimeout(2000);
    
    // Login with admin credentials
    console.log('Filling login form with admin@sledzspecke.pl...');
    await page.fill('[name="email"]', 'admin@sledzspecke.pl');
    await page.fill('[name="password"]', 'Test123');
    
    console.log('Clicking login button...');
    await page.click('button[type="submit"]');
    
    // Wait for navigation or error
    await page.waitForTimeout(3000);
    
    // Get auth token from localStorage
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    
    if (token) {
      console.log('✅ Admin login successful! Token received');
      
      // Decode JWT to check role
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('User role:', payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
      console.log('User ID:', payload.sub);
      
      // Navigate to dashboard
      const currentUrl = page.url();
      console.log('Current URL after login:', currentUrl);
      
      if (currentUrl.includes('/dashboard')) {
        console.log('✅ Successfully redirected to dashboard');
      }
    } else {
      console.log('❌ Login failed - no token');
      
      // Check for error messages
      const errorMessage = await page.$eval('.MuiAlert-message', el => el.textContent).catch(() => null);
      if (errorMessage) {
        console.log('Error message on page:', errorMessage);
      }
      
      // Take screenshot for debugging
      await page.screenshot({ path: 'login-failed.png' });
      console.log('Screenshot saved as login-failed.png');
    }
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();