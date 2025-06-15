const { chromium } = require('playwright');
const fs = require('fs');

(async () => {
    console.log('Creating demo video for Princess Agatka...');
    
    // Ensure directories exist
    const videoDir = '/var/www/sledzspecke-api/e2e-results/demo/chromium/videos';
    if (!fs.existsSync(videoDir)) {
        fs.mkdirSync(videoDir, { recursive: true });
    }
    
    const browser = await chromium.launch({
        headless: true
    });
    
    const context = await browser.newContext({
        recordVideo: {
            dir: videoDir,
            size: { width: 1280, height: 720 }
        },
        viewport: { width: 1280, height: 720 },
        locale: 'pl-PL'
    });
    
    const page = await context.newPage();
    
    console.log('Recording test: Princess Dashboard Demo...');
    
    // Navigate to Princess Dashboard
    await page.goto('https://api.sledzspecke.pl/princess-dashboard.html');
    await page.waitForTimeout(3000);
    
    // Take screenshot
    await page.screenshot({ path: `${videoDir}/../screenshots/princess-dashboard.png` });
    
    // Scroll through the page
    await page.evaluate(() => window.scrollBy(0, 300));
    await page.waitForTimeout(2000);
    
    // Navigate to SledzSpecke
    await page.goto('https://sledzspecke.pl');
    await page.waitForTimeout(3000);
    
    // Take screenshot
    await page.screenshot({ path: `${videoDir}/../screenshots/sledzspecke-home.png` });
    
    // Navigate to Google and search
    console.log('Recording test: Google search...');
    await page.goto('https://www.google.com');
    await page.waitForTimeout(2000);
    
    // Type in search
    await page.fill('textarea[name="q"]', 'Księżniczka Agatka ❤️');
    await page.waitForTimeout(3000);
    
    // Close to save video
    await context.close();
    await browser.close();
    
    // Wait for video to be saved
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Find and rename video file
    const files = fs.readdirSync(videoDir);
    const videoFile = files.find(f => f.endsWith('.webm'));
    
    if (videoFile) {
        const oldPath = `${videoDir}/${videoFile}`;
        const newPath = `${videoDir}/test-login-demo.webm`;
        fs.renameSync(oldPath, newPath);
        
        const stats = fs.statSync(newPath);
        console.log(`✅ Video created: ${newPath} (${Math.round(stats.size / 1024)} KB)`);
        
        // Fix permissions
        const { execSync } = require('child_process');
        execSync(`sudo chown -R www-data:www-data ${videoDir}`);
    } else {
        console.log('❌ No video file was created!');
    }
    
    console.log('Demo video creation completed!');
})();