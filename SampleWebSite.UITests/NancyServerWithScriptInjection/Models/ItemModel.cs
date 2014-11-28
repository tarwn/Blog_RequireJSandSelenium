using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests.NancyServerWithScriptInjection.Models
{
    public class ItemModel
    {
        public ItemModel(string id, string name, int inStock, double price, string summary)
        {
            Id = id;
            Name = name;
            InStock = inStock;
            Price = price;
            Summary = summary;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public double Price { get; set; }
        public string Summary { get; set; }
    }
}
