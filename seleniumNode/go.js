var Promise = require('bluebird');
var _ = require('lodash');
var resemble = require('resemblejs');

var configs = {
	server: {
		"exe":		__dirname + "/../SampleWebSite.UITests/bin/Debug/SampleWebSite.UITests.exe",
		"port":		"5001",
		"cwd":		__dirname + "/../SampleWebSite.UITests/bin/Debug",
		"url":		"http://localhost:5001/index.html"
	},
	phantom: {
		"address":	"http://localhost:4444"
	},
	resolutions: [
		{ name: "1920x1080", width: 1920, height: 1080, definitive: "screenshots/main_index_1920x1080_definitive.png" },
		{ name: "1024x768", width: 1024, height: 768, definitive: "screenshots/main_index_1024x768_definitive.png" }
	]
};

var webServer;

function startWebServer(){
	console.log('[==info==] Starting web server');
	return new Promise(function(resolve){
		var spawn = require('child_process').spawn;
		webServer = spawn(configs.server.exe, [configs.server.port], {
			cwd: configs.server.cwd
		});

		webServer.stdout.on('data', function(data){
			console.log("[==info==] [webServer stdout]: " + data);

			if(data.toString().indexOf('Server start: ready') > -1)
				resolve();
		});
		webServer.stderr.on('data', function (data) {
		  console.log('[==info==] [webServer stdout]: ' + data);
		});
	});
}

function performScreenshots(screenshotPath, url, resolution){
	console.log('[==info==] Perform screenshots starting');

	return new Promise(function(resolve, reject){
		var webdriver = require('selenium-webdriver'),
			By = require('selenium-webdriver').By,
			until = require('selenium-webdriver').until;

		var driver = new webdriver.Builder().usingServer(configs.phantom.address)
											.withCapabilities(webdriver.Capabilities.phantomjs())
											.build();

		driver.manage().window().setSize(resolution.width, resolution.height);

		console.log('[==info==] [script] Starting run');
		console.log('[==info==] [script] Loading ' + url);
		driver.get(url)
		.then(function(){
			driver.getTitle().then(function(title){
				console.log("[==info==] [script] Loaded: " + title);
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
					require('fs').writeFile(screenshotPath, image, 'base64', function(err) {
						//console.log('[script][error] ' + err);
					});
				});
			});
		})
		.then(function(){
				console.log('[==info==] [script] Finished run successfully');	
				console.log('[==info==] Screenshot done');
				driver.quit();
				resolve();
			}, 
			function(err){
				console.log('[==info==] [script] Finished run with error: ' + err);	
				console.log('[==info==] Screenshot done');
				driver.quit();
				reject(err);
			});
	});
}

function compareScreenshots(sourcePath, definitivePath, diffPath){
	// http://huddle.github.io/Resemble.js/
	console.log('[==info==] Comparing screenshots');
	
	return new Promise(function(resolve){
		resemble(sourcePath).compareTo(definitivePath)
							.ignoreAntialiasing()
							.onComplete(function(data){

			var diffImage = data.getImageDataUrl().replace(/^data:image\/png;base64,/,"");
			fs.writeFile(diffPath, diffImage, 'base64', function(err) {
				if (err) throw err;

				console.log('[==info==] Done comparing screenshots');
				resolve(data);
			});
		});
	});
}

function cleanUp(){
	console.log('[==info==] Clean up started');
	webServer.stdin.write('\n');
	webServer.stdin.end();
	console.log('[==info==] Clean up done');
}


var performAllScreenshots = function(){
	var screenshots = _.map(configs.resolutions, function(resolution){
		return performScreenshots('screenshots/index_' + resolution.name + '.png', configs.server.url, resolution);
	});
	return Promise.all(screenshots);
};

var compareAllScreenshots = function(){

};

startWebServer()
.then(performAllScreenshots)
.then(function(){ return compareScreenshots('screenshots/index_1920x1080.png', 'screenshots/index_1920x1080_definitive.png', 'screenshots/index_1920x1080_comparison.png'); })
.then(cleanUp);
