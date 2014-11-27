using SampleWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace SampleWebSite.Controllers
{
    public class ItemsController : ApiController
    {

        private List<ItemModel> _itemRepository = new List<ItemModel>() { 
            new ItemModel("BT-RC", "Red Tricycle w/ chrome wheels", 10, 75.99, "It's a red tricycle with chrome wheels, who doesn't want one?"),
            new ItemModel("BT-BC", "Blue Tricycle w/ chrome wheels", 5, 75.99, "It's a blue tricycle with chrome wheels, who doesn't want one?"),
            new ItemModel("B6-GA", "Grey 6-speed bike", 2, 125.45, "Like a 6-string guitar, but not"),
            new ItemModel("G6-G", "Grey 6-string guitar", 2, 150, "Actually a 6-string guitar"),
        };

        public IEnumerable<ItemSummaryModel> Get()
        {
            Thread.Sleep(3000);

            return _itemRepository.Select(im => new ItemSummaryModel(im));
        }

        public ItemModel Get(string id)
        {
            Thread.Sleep(1000);

            return _itemRepository.Where(im => im.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                                  .SingleOrDefault();
        }
        
    }
}