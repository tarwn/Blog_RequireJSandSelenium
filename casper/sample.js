phantom.casperPath = phantom.args[0];
phantom.injectJs(phantom.casperPath + "/bin/bootstrap.js");

var Promise = require('bluebird');

function startWebServer(){
	return new Promise(function(resolve){
		var spawn = require('child_process').spawn;
		var webServer = spawn('../SampleWebSite.UITests/bin/Debug/SampleWebSite.UITests.exe', ['5001'], {
			cwd: "../SampleWebSite.UITests/bin/Debug"
		});
		webServer.stdout.on('data', function(data){
			console.log("[webServer stdout]: " + data);

			if(data.indexOf('Server start: ready') > -1)
				resolve();
		});
		webServer.stderr.on('data', function (data) {
		  console.log('[webServer stdout]: ' + data);
		});
	});
}

function runCasper(){
	return new Promise(function(resolve){

		var casper = require('casper').create({
			logLevel:    "debug",
			verbose:     true,
			pageSettings: {
				loadPlugins: false
			}
		});

		// ----- casper actions start -----------------

		var screenshotResolutions = [
			{ width: 1280, height: 1014, name: '1280x1024' },
			{ width: 1920, height: 1080, name: '1920x1080' }
		];

		var url = 'http://localhost:5001/index.html';
		casper.start(url, function() {
			this.echo("[info][casper] Loaded '" + this.getTitle() + "' from '" + url + "'");

		});
			
		casper.eachThen(screenshotResolutions, function(resolution){
			var name = resolution.data.name;
			this.viewport(resolution.data.width, resolution.data.height)	
				.thenOpen(url)
				.wait(500)
				.capture('screenshots/index_' + name + '.png');
		});

		// ----- casper actions end -------------------

		casper.run(function(){
			// exit has a hard assumption about the context of 'this'
			resolve(this.exit.bind(this));
		});
	});
}

function cleanUp(casperExitCall){
	casperExitCall();
	phantom.exit();
	webServer.stdin.write('\n');
	webServer.stdin.end();
}

startWebServer().then(runCasper)
			    .then(cleanUp);
