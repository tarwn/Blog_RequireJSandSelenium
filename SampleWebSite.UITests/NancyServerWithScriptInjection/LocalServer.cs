using Nancy;
using SampleWebSite.UITests.NancyServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests.NancyServerWithScriptInjection
{
    public class LocalServer : NancyModule
    {
        private List<ItemModel> _items = new List<ItemModel>() { 
            new ItemModel("p1", "prod 1", 1, 1.99, "It's product 1"),
            new ItemModel("p2", "prod 2", 2, 2, "It's product 2"),
            new ItemModel("p3", "prod 3", 3, 3, "It's product 3")
        };

        public LocalServer()
        {
            Get["/api/items"] = parameters =>
            {
                return _items.Select(i => new ItemSummaryModel(i));
            };

            Get["/api/items/{id}"] = parameters =>
            {
                return _items.Where(i => i.Id.Equals(parameters.id, StringComparison.InvariantCultureIgnoreCase))
                             .SingleOrDefault();
            };
        }
    }

}
