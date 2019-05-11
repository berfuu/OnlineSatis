using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class MarkaController : Controller
    {
        private OnlineSatisDBEntities db = new OnlineSatisDBEntities();
        // GET: Marka
        public ActionResult Index()
        {

            return View(db.Marka.ToList());
        }
        [HttpGet]

        public ActionResult Create()
        {
            ViewBag.Markalar = new SelectList(db.Marka, "MarkaId", "Adi");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Marka marka, HttpPostedFileBase ResimURL)
        {

            db.Marka.Add(marka);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var b = db.Marka.Where(x => x.MarkaId == id).SingleOrDefault();
            if (b == null)
            {
                return HttpNotFound();
            }
            ViewBag.Marka = new SelectList(db.Marka, "MarkaId", "Adi", b.MarkaId);
            return View(b);
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Marka marka)
        {
            if (ModelState.IsValid)
            {
                var b = db.Marka.Where(x => x.MarkaId == id).SingleOrDefault();
                b.Adi = marka.Adi;
                b.Aciklama = marka.Aciklama;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(marka);
        }
       

        
        public ActionResult Delete(int id)
        {
            var finded = db.Marka.Find(id);//Siz de uyuyonuz valla ha sizi deniyorum : hayır kabul etmiyorum burda söyledim ben ama sana değil:))
            if (finded == null) return null;
            //Silindiği taktirde aynı galeriye dönebilsin.
            //Sizi deniyorum ben :D bak mesela silmemişiz resmi ? kimse uyarmadı
            //biz bunu gördük ama sen bir şeyler yapıyorsun diye araya giremedik remove yapmadık :)))
            db.Marka.Remove(finded);
            db.SaveChanges();
            return RedirectToAction("Index", "Marka", new { MarkaId = id });
        }
    }
}