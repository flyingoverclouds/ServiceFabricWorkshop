using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppliWeb.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var svc = this.HttpContext.RequestServices.GetService(typeof(System.Fabric.StatelessServiceContext)) as System.Fabric.StatelessServiceContext;
            ViewData["Message"] = $"Page render on node {Environment.MachineName} <br/>  ServiceNAme is [{svc.ServiceName}] <br/>  Partition ID is [{svc.PartitionId}] <br/>  Instance Id is [{svc.ReplicaOrInstanceId}]";
            
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
