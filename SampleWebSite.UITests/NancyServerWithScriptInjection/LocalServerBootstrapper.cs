using Nancy;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests.NancyServerWithScriptInjection
{
    public class LocalServerBootstrapper : DefaultNancyBootstrapper
    {
        private string _serverFolder;

        public LocalServerBootstrapper(string serverFolder)
        {
            _serverFolder = serverFolder;
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            //overrides
            conventions.StaticContentsConventions.AddFile("/Scripts/app/services/itemService.js", "FakeScripts/app/services/itemService.js");

            conventions.StaticContentsConventions.AddFile("/index.html", _serverFolder + "/index.html");
            conventions.StaticContentsConventions.AddDirectory("/Scripts", _serverFolder + "/Scripts");
            conventions.StaticContentsConventions.AddDirectory("/style", _serverFolder + "/style");
        }
    }
}
