var Promise = require('bluebird');

var configs = {
	server: {
		"exe":		__dirname + "/../SampleWebSite.UITests/bin/Debug/SampleWebSite.UITests.exe",
		"port":		"5001",
		"cwd":		__dirname + "/../SampleWebSite.UITests/bin/Debug",
		"url":		"http://localhost:5001/index.html"
	},
	phantom: {
		"address":	"http://localhost:4444"
	}
};

var webServer;

function startWebServer(){
	return new Promise(function(resolve){
		var spawn = require('child_process').spawn;
		webServer = spawn(configs.server.exe, [configs.server.port], {
			cwd: configs.server.cwd
		});

		webServer.stdout.on('data', function(data){
			console.log("[webServer stdout]: " + data);

			if(data.toString().indexOf('Server start: ready') > -1)
				resolve();
		});
		webServer.stderr.on('data', function (data) {
		  console.log('[webServer stdout]: ' + data);
		});
	});
}

function performScreenshots(pageName, url){

	return new Promise(function(resolve, reject){
		var webdriver = require('selenium-webdriver'),
			By = require('selenium-webdriver').By,
			until = require('selenium-webdriver').until;

		var driver = new webdriver.Builder().usingServer(configs.phantom.address)
											.withCapabilities(webdriver.Capabilities.phantomjs())
											.build();

		driver.manage().window().setSize(1920,1080);
		var resolutionName = "1920x1080";

		console.log('[script] Starting run');
		console.log('[script] Loading ' + url);
		driver.get(url)
		.then(function(){
			driver.getTitle().then(function(title){
				console.log("[script] Loaded: " + title);
			});

			// get the page set the right way
			// - click the search button
			driver.findElement(webdriver.By.id('search-button')).then(function(button){
				button.click();
			});

			// - wait for results, then click the add button
			driver.wait(function(){
				// promises all the way down, supposed to wait for resolution
				return driver.findElement(webdriver.By.css('table.search-results-items')).then(function(elem){
					return elem.isDisplayed();
				});
			}, 5000)
			.then(function(){
				driver.findElement(webdriver.By.css('.add-to-cart-button')).then(function(button){
					button.click();
				});
			});

			// - wait for details and take screenshot
			driver.wait(function(){
				// promises all the way down, supposed to wait for resolution
				return driver.findElement(webdriver.By.id('item-details')).then(function(elem){
					return elem.isDisplayed();
				});
			}, 5000)
			.then(function(){
				// screenshot
				driver.takeScreenshot().then(function(image, err) {
					require('fs').writeFile('screenshots/' + pageName + '_' + resolutionName + '.png', image, 'base64', function(err) {
						console.log('[script][error] ' + err);
					});
				});
			});
		})
		.then(function(){
				console.log('[script] Finished run successfully');	
				console.log('[script] Done');
				driver.quit();
				resolve();
			}, 
			function(err){
				console.log('[script] Finished run with error: ' + err);	
				console.log('[script] Done');
				driver.quit();
				reject(err);
			});
	});
}

function compareScreenshots(){
	// http://huddle.github.io/Resemble.js/

	return new Promise(resolve){
		resolve();
	};
}

function cleanUp(){
	webServer.stdin.write('\n');
	webServer.stdin.end();
}

startWebServer()
.then(function(){ return performScreenshots('index', configs.server.url); })
.then(compareScreenshots)
.then(cleanUp);