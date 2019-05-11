using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class TestController : Controller
    {
       
        // GET: Test
        public ActionResult Index()
        {
            //OnlineSatisDBEntities db = new OnlineSatisDBEntities();

            //var varyant=new Models.DB.UrunVaryant();
            //varyant.Label = "İşlemci";
            //varyant.Value = "2 GB";
            //varyant.UrunId = 4;
            //db.UrunVaryant.Add(varyant);
            //db.SaveChanges();
            return View();

        }
        
    }
}