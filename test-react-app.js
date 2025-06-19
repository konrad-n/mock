#!/usr/bin/env node

const puppeteer = require('puppeteer');

(async () => {
  const browser = await puppeteer.launch({
    headless: 'new',
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });
  
  try {
    const page = await browser.newPage();
    
    // Listen for console messages
    page.on('console', msg => {
      console.log('Browser console:', msg.type(), msg.text());
    });
    
    // Listen for page errors
    page.on('pageerror', error => {
      console.log('Page error:', error.message);
    });
    
    // Navigate to login page
    console.log('Navigating to https://sledzspecke.pl/login');
    await page.goto('https://sledzspecke.pl/login', { 
      waitUntil: 'networkidle2',
      timeout: 30000 
    });
    
    // Wait a bit for React to render
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Check page content
    const html = await page.content();
    console.log('\nPage HTML (first 500 chars):');
    console.log(html.substring(0, 500));
    
    // Check for React root
    const rootExists = await page.$('#root');
    console.log('\nReact root element exists:', !!rootExists);
    
    // Check for login form
    const formExists = await page.$('form');
    console.log('Form element exists:', !!formExists);
    
    // Check for any visible text
    const bodyText = await page.evaluate(() => document.body.innerText);
    console.log('\nVisible text on page:');
    console.log(bodyText);
    
    // Take screenshot
    await page.screenshot({ path: 'login-page.png', fullPage: true });
    console.log('\nScreenshot saved as login-page.png');
    
  } finally {
    await browser.close();
  }
})();