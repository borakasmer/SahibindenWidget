using DAL;
using Microsoft.AspNet.SignalR;
using SahibindenBanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace SahibindenBanner.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var data = GetCars();  
                return View(data);                      
        }

        public List<SahibindenCars> GetCars()
        {
            if (Request.QueryString["clear"] == "1") { HttpContext.Cache.Remove("Cars"); }
            
            if (HttpContext.Cache["Cars"] == null)
                {
                    using (Banner dbContext = new Banner())
                    {             
                        HttpContext.Cache.Add("Cars", dbContext.SahibindenCars.ToList(), null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), CacheItemPriority.Default, null);
                    }
                }            
            // Bitti Cache
            return ((List<SahibindenCars>)HttpContext.Cache["Cars"]);
        }
    }

    public class GetCars : Hub
    {
        public void RefreshCars()
        {
            List<DAL.SahibindenCars> data;
            using (Banner dbContext = new Banner())
            {
                data=dbContext.SahibindenCars.ToList();
            }
            string viewName = @"~/Views/Shared/SahibindenCars.cshtml";
            var carsScreenPrint = Helper.GetRazorViewAsString(data, viewName);
            Clients.All.reloadCars(carsScreenPrint);
        }
    }

}