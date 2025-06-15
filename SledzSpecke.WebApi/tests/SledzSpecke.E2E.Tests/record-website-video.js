const { chromium } = require('@playwright/test');
const fs = require('fs');
const path = require('path');

async function recordWebsiteVideo() {
  const resultsDir = 'Reports/Videos/latest';
  const archiveDir = 'Reports/Videos/archive';
  
  // Clean up old recordings - move to archive
  if (fs.existsSync(resultsDir)) {
    if (!fs.existsSync(archiveDir)) {
      fs.mkdirSync(archiveDir, { recursive: true });
    }
    
    // Delete old archives to save space (keep only last 2)
    const archives = fs.readdirSync(archiveDir).sort().reverse();
    archives.slice(2).forEach(dir => {
      fs.rmSync(path.join(archiveDir, dir), { recursive: true, force: true });
    });
    
    // Move current to archive with timestamp
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    fs.renameSync(resultsDir, path.join(archiveDir, timestamp));
  }
  
  fs.mkdirSync(resultsDir, { recursive: true });
  
  const browser = await chromium.launch({ 
    headless: true, // Use headless mode for server
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });
  
  const context = await browser.newContext({
    viewport: { width: 1280, height: 720 },
    locale: 'pl-PL',
    ignoreHTTPSErrors: true,
    recordVideo: {
      dir: resultsDir,
      size: { width: 1280, height: 720 }
    }
  });
  
  const page = await context.newPage();
  const testResults = [];
  
  try {
    console.log('🎬 Starting video recording...');
    
    // Test 1: Homepage Load
    console.log('\n📍 Test 1: Loading homepage...');
    const startTime = Date.now();
    try {
      await page.goto('https://sledzspecke.pl', { 
        waitUntil: 'domcontentloaded', 
        timeout: 30000 
      });
      await page.waitForTimeout(3000); // Let user see the page
      testResults.push({
        name: 'Homepage Load',
        status: 'success',
        duration: Date.now() - startTime,
        timestamp: new Date().toISOString()
      });
      console.log('✅ Homepage loaded successfully');
    } catch (e) {
      testResults.push({
        name: 'Homepage Load',
        status: 'failed',
        error: e.message,
        duration: Date.now() - startTime,
        timestamp: new Date().toISOString()
      });
      console.log('❌ Homepage failed to load:', e.message);
    }
    
    // Test 2: Navigation to Login
    console.log('\n📍 Test 2: Navigating to login...');
    const loginStart = Date.now();
    try {
      // Try multiple selectors
      const loginSelectors = [
        'text=Zaloguj się',
        'text=Zaloguj',
        'text=Login',
        'a[href*="login"]',
        'button:has-text("Zaloguj")',
        '[data-test="login-button"]'
      ];
      
      let clicked = false;
      for (const selector of loginSelectors) {
        try {
          await page.click(selector, { timeout: 3000 });
          clicked = true;
          console.log(`✅ Clicked login using: ${selector}`);
          break;
        } catch (e) {
          // Try next selector
        }
      }
      
      if (!clicked) {
        throw new Error('Could not find login button');
      }
      
      await page.waitForTimeout(3000);
      testResults.push({
        name: 'Navigate to Login',
        status: 'success',
        duration: Date.now() - loginStart,
        timestamp: new Date().toISOString()
      });
    } catch (e) {
      testResults.push({
        name: 'Navigate to Login',
        status: 'failed',
        error: e.message,
        duration: Date.now() - loginStart,
        timestamp: new Date().toISOString()
      });
      console.log('❌ Could not navigate to login:', e.message);
    }
    
    // Test 3: Login Form Interaction
    console.log('\n📍 Test 3: Testing login form...');
    const formStart = Date.now();
    try {
      // Type slowly so it's visible in video
      await page.fill('input[name="username"]', 'testuser', { timeout: 5000 });
      await page.waitForTimeout(1000);
      await page.fill('input[name="password"]', 'Test123!', { timeout: 5000 });
      await page.waitForTimeout(1000);
      
      // Find and click submit button
      await page.click('button[type="submit"]', { timeout: 5000 });
      await page.waitForTimeout(3000);
      
      testResults.push({
        name: 'Login Form Interaction',
        status: 'success',
        duration: Date.now() - formStart,
        timestamp: new Date().toISOString()
      });
      console.log('✅ Login form filled successfully');
    } catch (e) {
      testResults.push({
        name: 'Login Form Interaction',
        status: 'failed',
        error: e.message,
        duration: Date.now() - formStart,
        timestamp: new Date().toISOString()
      });
      console.log('❌ Login form interaction failed:', e.message);
    }
    
    // Test 4: API Documentation
    console.log('\n📍 Test 4: Checking API documentation...');
    const apiStart = Date.now();
    try {
      await page.goto('https://api.sledzspecke.pl/swagger', { 
        waitUntil: 'domcontentloaded', 
        timeout: 30000 
      });
      await page.waitForTimeout(3000);
      
      // Try to expand an endpoint
      await page.click('.opblock-summary', { timeout: 5000 });
      await page.waitForTimeout(2000);
      
      testResults.push({
        name: 'API Documentation',
        status: 'success',
        duration: Date.now() - apiStart,
        timestamp: new Date().toISOString()
      });
      console.log('✅ API documentation loaded successfully');
    } catch (e) {
      testResults.push({
        name: 'API Documentation',
        status: 'failed',
        error: e.message,
        duration: Date.now() - apiStart,
        timestamp: new Date().toISOString()
      });
      console.log('❌ API documentation failed:', e.message);
    }
    
    // Test 5: E2E Dashboard (when deployed)
    console.log('\n📍 Test 5: Checking E2E Dashboard...');
    const dashboardStart = Date.now();
    try {
      await page.goto('https://api.sledzspecke.pl/e2e-dashboard', { 
        waitUntil: 'domcontentloaded', 
        timeout: 30000 
      });
      await page.waitForTimeout(3000);
      
      testResults.push({
        name: 'E2E Dashboard',
        status: 'success',
        duration: Date.now() - dashboardStart,
        timestamp: new Date().toISOString()
      });
      console.log('✅ E2E Dashboard loaded successfully');
    } catch (e) {
      testResults.push({
        name: 'E2E Dashboard',
        status: 'failed',
        error: e.message,
        duration: Date.now() - dashboardStart,
        timestamp: new Date().toISOString()
      });
      console.log('❌ E2E Dashboard not available yet');
    }
    
  } finally {
    // Close browser to save video
    await context.close();
    await browser.close();
    
    console.log('\n🎬 Video recording complete!');
    
    // Wait for video to be saved
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Find the video file
    const videos = fs.readdirSync(resultsDir).filter(f => f.endsWith('.webm'));
    const videoFile = videos[0] || 'video.webm';
    
    // Generate HTML report
    const reportHtml = `<!DOCTYPE html>
<html lang="pl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SledzSpecke - E2E Video Test Results</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            margin: 0;
            padding: 20px;
            background: #f5f7fa;
            color: #333;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
        }
        h1 {
            color: #2c3e50;
            text-align: center;
            margin-bottom: 10px;
        }
        .subtitle {
            text-align: center;
            color: #7f8c8d;
            margin-bottom: 30px;
        }
        .video-container {
            background: white;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        video {
            width: 100%;
            max-width: 1280px;
            height: auto;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.15);
        }
        .test-results {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .test-item {
            padding: 15px;
            margin-bottom: 10px;
            border-radius: 8px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-left: 4px solid transparent;
        }
        .test-item.success {
            background: #d4edda;
            border-left-color: #28a745;
        }
        .test-item.failed {
            background: #f8d7da;
            border-left-color: #dc3545;
        }
        .test-name {
            font-weight: 600;
        }
        .test-duration {
            color: #6c757d;
            font-size: 14px;
        }
        .summary {
            background: #e3f2fd;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            text-align: center;
        }
        .summary-stats {
            display: flex;
            justify-content: center;
            gap: 30px;
            margin-top: 15px;
        }
        .stat {
            text-align: center;
        }
        .stat-number {
            font-size: 36px;
            font-weight: bold;
            display: block;
        }
        .stat-label {
            color: #666;
            font-size: 14px;
        }
        .success-stat { color: #28a745; }
        .failed-stat { color: #dc3545; }
        .info-box {
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
        }
        @media (max-width: 768px) {
            .summary-stats {
                flex-direction: column;
                gap: 15px;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>🎬 SledzSpecke - E2E Video Test Results</h1>
        <p class="subtitle">Test wykonany: ${new Date().toLocaleString('pl-PL')}</p>
        
        <div class="video-container">
            <h2>📹 Nagranie z testów</h2>
            <video controls autoplay muted>
                <source src="${videoFile}" type="video/webm">
                Twoja przeglądarka nie obsługuje odtwarzania wideo.
            </video>
        </div>
        
        <div class="test-results">
            <h2>📊 Wyniki testów</h2>
            
            <div class="summary">
                <h3>Podsumowanie</h3>
                <div class="summary-stats">
                    <div class="stat">
                        <span class="stat-number">${testResults.length}</span>
                        <span class="stat-label">Wszystkie testy</span>
                    </div>
                    <div class="stat">
                        <span class="stat-number success-stat">${testResults.filter(t => t.status === 'success').length}</span>
                        <span class="stat-label">Zaliczone</span>
                    </div>
                    <div class="stat">
                        <span class="stat-number failed-stat">${testResults.filter(t => t.status === 'failed').length}</span>
                        <span class="stat-label">Niezaliczone</span>
                    </div>
                </div>
            </div>
            
            <div class="info-box">
                <strong>ℹ️ Informacja:</strong> Nagranie pokazuje próbę automatycznego testowania strony SledzSpecke. 
                Testy sprawdzają czy strona się ładuje, czy można się zalogować i czy API działa poprawnie.
            </div>
            
            <h3>Szczegóły testów:</h3>
            ${testResults.map(test => `
                <div class="test-item ${test.status}">
                    <div>
                        <div class="test-name">${test.status === 'success' ? '✅' : '❌'} ${test.name}</div>
                        ${test.error ? `<div style="color: #721c24; font-size: 14px; margin-top: 5px;">${test.error}</div>` : ''}
                    </div>
                    <div class="test-duration">${(test.duration / 1000).toFixed(1)}s</div>
                </div>
            `).join('')}
        </div>
    </div>
</body>
</html>`;
    
    fs.writeFileSync(path.join(resultsDir, 'index.html'), reportHtml);
    
    // Save JSON results
    const jsonResults = {
      timestamp: new Date().toISOString(),
      videoFile: videoFile,
      testResults: testResults,
      summary: {
        total: testResults.length,
        passed: testResults.filter(t => t.status === 'success').length,
        failed: testResults.filter(t => t.status === 'failed').length
      }
    };
    
    fs.writeFileSync(
      path.join(resultsDir, 'results.json'),
      JSON.stringify(jsonResults, null, 2)
    );
    
    console.log(`\n📁 Results saved to: ${resultsDir}`);
    console.log(`📺 Open ${path.join(resultsDir, 'index.html')} to view the video and results`);
  }
}

// Run if called directly
if (require.main === module) {
  recordWebsiteVideo().catch(console.error);
}

module.exports = { recordWebsiteVideo };