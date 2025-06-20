const { chromium } = require('playwright');

(async () => {
  console.log('Checking login page structure...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext();
  const page = await context.newPage();

  try {
    // Go to login page
    console.log('Navigating to https://sledzspecke.pl...');
    await page.goto('https://sledzspecke.pl', { waitUntil: 'networkidle' });
    
    // Take screenshot
    await page.screenshot({ path: 'login-page-check.png' });
    console.log('Screenshot saved as login-page-check.png');
    
    // Check page content
    const pageContent = await page.content();
    console.log('\nPage title:', await page.title());
    
    // Look for login form elements
    const emailInput = await page.$('[type="email"], [name="email"], input[placeholder*="mail" i]');
    const passwordInput = await page.$('[type="password"], [name="password"], input[placeholder*="asło" i]');
    const loginButton = await page.$('button:has-text("Zaloguj"), button:has-text("Login"), button[type="submit"]');
    
    console.log('\nForm elements found:');
    console.log('Email input:', emailInput !== null);
    console.log('Password input:', passwordInput !== null);
    console.log('Login button:', loginButton !== null);
    
    // Get actual selectors
    if (emailInput) {
      const emailSelector = await emailInput.evaluate(el => {
        const attrs = [];
        if (el.id) attrs.push(`#${el.id}`);
        if (el.name) attrs.push(`[name="${el.name}"]`);
        if (el.placeholder) attrs.push(`[placeholder="${el.placeholder}"]`);
        if (el.type) attrs.push(`[type="${el.type}"]`);
        return attrs.join(' or ');
      });
      console.log('Email selector:', emailSelector);
    }
    
    if (passwordInput) {
      const passwordSelector = await passwordInput.evaluate(el => {
        const attrs = [];
        if (el.id) attrs.push(`#${el.id}`);
        if (el.name) attrs.push(`[name="${el.name}"]`);
        if (el.placeholder) attrs.push(`[placeholder="${el.placeholder}"]`);
        if (el.type) attrs.push(`[type="${el.type}"]`);
        return attrs.join(' or ');
      });
      console.log('Password selector:', passwordSelector);
    }
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();