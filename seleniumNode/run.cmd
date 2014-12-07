start /B ..\packages\PhantomJS.1.9.8\tools\phantomjs\phantomjs.exe --webdriver=4444
node go.js
taskkill /IM "phantomjs.exe" /f