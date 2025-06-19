#!/usr/bin/env node

const puppeteer = require('puppeteer');

(async () => {
  const browser = await puppeteer.launch({
    headless: 'new',
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });
  
  try {
    const page = await browser.newPage();
    
    const errors = [];
    const warnings = [];
    const logs = [];
    
    // Listen for console messages
    page.on('console', msg => {
      const type = msg.type();
      const text = msg.text();
      
      if (type === 'error') {
        errors.push(text);
      } else if (type === 'warning') {
        warnings.push(text);
      } else {
        logs.push(`[${type}] ${text}`);
      }
    });
    
    // Listen for page errors
    page.on('pageerror', error => {
      errors.push(`PAGE ERROR: ${error.message}`);
    });
    
    // Listen for request failures
    page.on('requestfailed', request => {
      errors.push(`REQUEST FAILED: ${request.url()} - ${request.failure().errorText}`);
    });
    
    // Navigate to login page
    console.log('Navigating to https://sledzspecke.pl/login');
    await page.goto('https://sledzspecke.pl/login', { 
      waitUntil: 'networkidle2',
      timeout: 30000 
    });
    
    // Wait for any async operations
    await new Promise(resolve => setTimeout(resolve, 3000));
    
    // Print results
    console.log('\n=== Console Errors ===');
    if (errors.length === 0) {
      console.log('No errors found');
    } else {
      errors.forEach(err => console.log('ERROR:', err));
    }
    
    console.log('\n=== Console Warnings ===');
    if (warnings.length === 0) {
      console.log('No warnings found');
    } else {
      warnings.forEach(warn => console.log('WARNING:', warn));
    }
    
    console.log('\n=== Console Logs ===');
    if (logs.length === 0) {
      console.log('No logs found');
    } else {
      logs.forEach(log => console.log(log));
    }
    
    // Test the login functionality
    console.log('\n=== Testing Login Form ===');
    
    // Fill in the form
    await page.type('input[name="email"]', 'testuser');
    await page.type('input[name="password"]', 'Test123!');
    
    // Click login button
    await page.click('button[type="submit"]');
    
    // Wait for response
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Check current URL
    const currentUrl = page.url();
    console.log('Current URL after login:', currentUrl);
    
    // Check for error message
    const errorAlert = await page.$('.MuiAlert-root');
    if (errorAlert) {
      const errorText = await page.evaluate(el => el.textContent, errorAlert);
      console.log('Error message:', errorText);
    }
    
  } finally {
    await browser.close();
  }
})();