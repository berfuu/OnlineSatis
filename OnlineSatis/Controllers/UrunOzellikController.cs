using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class UrunOzellikController : Controller
    {
        private  OnlineSatisDBEntities db=new OnlineSatisDBEntities();
        // GET: UrunOzellik
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false; //join işlemi yapmadan o tabloya ait istediğimiz tabloların bilgilerini de getirebilmek için kullanılıyor.
            return View(db.UrunVaryant.ToList().OrderByDescending(x=>x.Id));
        }

        public ActionResult Create()
        {
            ViewBag.Urun = new SelectList(db.Urun, "UrunId", "Adi");
            return View(new UrunVaryant());
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UrunVaryant urun)
        {
            db.UrunVaryant.Add(urun);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (id==null)
            {
                return HttpNotFound();
            }

            var u = db.UrunVaryant.Where(x => x.Id == id).SingleOrDefault();
            if (u==null)
            {
                return HttpNotFound();
            }
            ViewBag.Urun = new SelectList(db.Urun, "UrunId", "Adi");
            return View(u);
        }
        [HttpPost]
        public ActionResult Edit(int id ,UrunVaryant urun)
        {
            if (ModelState.IsValid)
            {
                var u = db.UrunVaryant.Where(x => x.Id == id).SingleOrDefault();
                u.Label = urun.Label;
                u.Value = urun.Value;
                u.UrunId = urun.UrunId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(urun);

        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrunVaryant urunVaryant = db.UrunVaryant.Find(id);
            if (urunVaryant == null)
            {
                return HttpNotFound();
            }
            return View(urunVaryant);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var finded = db.UrunVaryant.Find(id);
            if (finded == null) return null;
            db.UrunVaryant.Remove(finded);
            db.SaveChanges();
            return RedirectToAction("Index", "UrunOzellik", new { UrunVaryant = id });
        }
    }
}