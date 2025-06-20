const { chromium } = require('playwright');

(async () => {
  console.log('Running data migration through admin panel...');
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext();
  const page = await context.newPage();

  try {
    // Login as admin
    console.log('Logging in as admin...');
    await page.goto('https://sledzspecke.pl');
    await page.fill('[name="email"]', 'admin@sledzspecke.pl');
    await page.fill('[name="password"]', 'Test123');
    await page.click('button[type="submit"]');
    await page.waitForTimeout(2000);
    
    // Get auth token
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    if (!token) {
      console.log('❌ Failed to login as admin');
      return;
    }
    
    console.log('✅ Logged in successfully');
    
    // Check migration status
    console.log('\nChecking migration status...');
    const statusResponse = await page.evaluate(async (token) => {
      const res = await fetch('https://api.sledzspecke.pl/api/admin/data-migration/procedures/status', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      return res.json();
    }, token);
    
    console.log('Migration status:', statusResponse);
    
    if (statusResponse.migrationNeeded) {
      console.log('\n🔄 Running procedure migration...');
      
      const migrationResponse = await page.evaluate(async (token) => {
        const res = await fetch('https://api.sledzspecke.pl/api/admin/data-migration/procedures', {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
        return {
          status: res.status,
          body: await res.json()
        };
      }, token);
      
      console.log('Migration response status:', migrationResponse.status);
      console.log('Migration result:', migrationResponse.body);
      
      if (migrationResponse.body.success) {
        console.log('\n✅ Migration completed successfully!');
        console.log('Statistics:', migrationResponse.body.statistics);
      } else {
        console.log('❌ Migration failed:', migrationResponse.body.message);
      }
    } else {
      console.log('\n✅ Migration already completed - no action needed');
    }
    
  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();