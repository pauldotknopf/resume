const puppeteer = require('puppeteer');

(async () => {
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto('http://localhost:8000/template');
  //await page.emulateMedia('print');
  await page.pdf({path: 'output.pdf'});
  await browser.close();
})();