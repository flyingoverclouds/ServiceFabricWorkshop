using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SvcStatefull1.Interfaces;
using Microsoft.ServiceFabric.Services.Client;

namespace Web1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("shop/{shopId?}")]
        public async Task<IActionResult> About(string shopId)
        {
   

            var stockSvc = ServiceProxy.Create<IStockService>(new Uri("fabric:/HolSFstep3/StockShop" + shopId), 
                new ServicePartitionKey(0));

            var qtySurfBook = await stockSvc.GetStockForProduct("SurfaceBook");
            await stockSvc.UpdateStockForProduct("SurfaceBook", -1);
            
            ViewData["Message"] = $"Il y a {qtySurfBook} SurfaceBook en stock";
            
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
