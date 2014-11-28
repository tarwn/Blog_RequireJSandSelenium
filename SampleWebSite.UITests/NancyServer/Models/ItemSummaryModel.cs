using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests.NancyServer.Models
{
    public class ItemSummaryModel
    {
        public ItemSummaryModel(ItemModel item)
        {
            Id = item.Id;
            Name = item.Name;
            InStock = item.InStock;
            Price = item.Price;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public double Price { get; set; }
    }
}
