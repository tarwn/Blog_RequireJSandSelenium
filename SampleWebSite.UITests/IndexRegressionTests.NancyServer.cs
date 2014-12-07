using Nancy.Hosting.Self;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using Owin;
using SampleWebSite.UITests.NancyServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{

    //[TestFixture]
    public class IndexRegressionTests_NancyServer
    {

        static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.Write("I require an argument for the local port number");
                return;
            }

            int port;
            if (!int.TryParse(args[0], out port)) {
                Console.WriteLine("The non-integer value '" + args[0] + "' is a poror port number");
                return;
            }

            Console.WriteLine("Starting server...");
            var webServer = SetupServer(port);

            Console.WriteLine("Press any key to exit...");
            Console.Read();

            Console.WriteLine("Stopping server...");
            webServer.Stop();
            Console.WriteLine("Done.");
        }

        private static NancyHost SetupServer(int portNumber)
        {
            string baseExePath = Assembly.GetExecutingAssembly().Location;
            var fileInfo = new FileInfo(baseExePath);
            string basePath = fileInfo.Directory.FullName.Replace("\\", "/");

            Console.WriteLine("Server start: copying files");
            var dnfo = new DirectoryInfo(basePath + "/TestSampleWebSite");
            if (dnfo.Exists)
                dnfo.Delete(true);

            var proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = @"C:\WINDOWS\system32\xcopy.exe";
            proc.StartInfo.Arguments = String.Format("\"{0}/../../../SampleWebSite\" \"{0}/TestSampleWebSite\" /E /I /F", basePath);
            Console.WriteLine("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
            proc.Start();
            proc.WaitForExit();

            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations()
                {
                    User = "Everyone",
                    CreateAutomatically = true
                }
            };

            Console.WriteLine("Server start: starting host");
            var host = new NancyHost(new LocalServerBootstrapper(dnfo.FullName), config, new Uri("http://localhost:" + portNumber.ToString()));
            host.Start();

            Console.WriteLine("Server start: ready");
            return host;
        }

    }
}
