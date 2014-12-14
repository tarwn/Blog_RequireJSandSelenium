Using Selenium for View testing
=================================

I built this as an exploration of a ideas on testing the View of a basic SPA-like site. The goal 
is finding a way to utilize Selenium to test that my bindings and View are interacting as 
expected, without bringing in the extra overhead of having the front-end talk to a live site/API.

Blog Post: [Using Selenium for View testing with knockout and RequireJS](http://blogs.lessthandot.com/index.php/webdev/using-selenium-for-view-testing-with-knockout-and-requirejs/)

The best methods, of the ones I explored, used Nancy to self-host static files and provide
fake API endpoints or fake service files. The fake API method seemed the most promising, as you 
could manage the test data responses specific for each test.

Screenshot Regression
=======================

I played with screenshot regression testing for a couple weekends, but just ended up really
annoyed. Tools like Casper and Resemble.js seem really promising, but I had difficulty wrapping
my brain around casper's built-in async model, then when I used selenium with resemble I had
the same issue getting my head around seleniums's built-in async model, then ran out of steam
when I realized resemble needed a document object to create a canvas for comparisons.

I'm at the point where I could finish either of them, and have some thoughts on a few other 
non-node.js methods, but do not want to finish them, as they feel like they would be great demos
that require a high level of mainatenence when implemented for a real deployment.

Along the way, I did add some methods to make it posible to run the self-hosted Nancy version
from command-line, which was the easiest part of the whole experiment and could be useful
in other ways.
