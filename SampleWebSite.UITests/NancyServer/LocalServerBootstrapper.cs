using Nancy;
using Nancy.Conventions;
using Nancy.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests.NancyServer
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

            Console.WriteLine("Configuring static files at: " + _serverFolder);

            conventions.StaticContentsConventions.AddFile("/index.html", _serverFolder + "/index.html");
            conventions.StaticContentsConventions.AddDirectory("/Scripts", _serverFolder + "/Scripts");
            conventions.StaticContentsConventions.AddDirectory("/style", _serverFolder + "/style");
        }
    }
}
