const { chromium } = require('playwright');

(async () => {
  console.log('Testing admin login with detailed debugging...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext();
  const page = await context.newPage();

  // Enable console logging
  page.on('console', msg => console.log('Browser console:', msg.text()));
  
  // Enable request/response logging
  page.on('request', request => {
    if (request.url().includes('/api/')) {
      console.log('API Request:', request.method(), request.url());
    }
  });
  
  page.on('response', response => {
    if (response.url().includes('/api/')) {
      console.log('API Response:', response.status(), response.url());
    }
  });

  try {
    // Go to login page
    console.log('Navigating to https://sledzspecke.pl...');
    await page.goto('https://sledzspecke.pl', { waitUntil: 'networkidle' });
    
    // Take screenshot before login
    await page.screenshot({ path: 'before-login.png' });
    
    // Login with admin credentials
    console.log('Filling login form with admin@sledzspecke.pl...');
    await page.fill('[name="email"]', 'admin@sledzspecke.pl');
    await page.fill('[name="password"]', 'Test123!');
    
    console.log('Clicking login button...');
    await page.click('button[type="submit"]');
    
    // Wait for response
    try {
      const response = await page.waitForResponse(
        response => response.url().includes('/api/auth/sign-in'),
        { timeout: 5000 }
      );
      console.log('Login response status:', response.status());
      const responseBody = await response.text();
      console.log('Login response body:', responseBody);
    } catch (e) {
      console.log('No login response received within 5 seconds');
    }
    
    // Wait a bit more
    await page.waitForTimeout(2000);
    
    // Take screenshot after login attempt
    await page.screenshot({ path: 'after-login.png' });
    
    // Check current URL
    console.log('Current URL:', page.url());
    
    // Get auth token from localStorage
    const token = await page.evaluate(() => localStorage.getItem('auth_token'));
    if (token) {
      console.log('✅ Token found in localStorage');
      
      // Decode JWT to check role
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('JWT payload:', JSON.stringify(payload, null, 2));
    } else {
      console.log('❌ No token in localStorage');
      
      // Check for error messages
      const errorMessage = await page.$eval('.error-message, .alert-danger, [role="alert"]', el => el.textContent).catch(() => null);
      if (errorMessage) {
        console.log('Error message on page:', errorMessage);
      }
    }
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();