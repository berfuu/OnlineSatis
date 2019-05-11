﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class UrunlerController : Controller
    {
        private OnlineSatisDBEntities db = new OnlineSatisDBEntities();

        // GET: Urunler
        public ActionResult Index()
        {
            return View(db.Urun.ToList());
        }

        public ActionResult Create()
        {

            ViewBag.Kategoriler =new SelectList(db.Kategori, "KategoriId", "Adi"); // contoller den view veri taşımak için kullanılır.
            ViewBag.Markalar = new SelectList(db.Marka, "MarkaId", "Adi");
            return View(new Urun());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Urun urun, List<HttpPostedFileBase> BrandImg)
        {
            urun.EklenmeTarihi = DateTime.Now;
            db.Urun.Add(urun);
            db.SaveChanges();
            var b = db.Urun.OrderByDescending(x => x.UrunId).FirstOrDefault();

            if (BrandImg != null )
            {
                foreach (var item in BrandImg)
                {
                    WebImage img = new WebImage(item.InputStream);
                    FileInfo imginfo = new FileInfo(item.FileName);

                    string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    string imgPath = "/Uploads/Urun/" + urunimgname;
                    img.Save(imgPath);
                    db.Resim.Add(new Resim()
                    {
                        ResimURL = imgPath,
                        UrunId = b.UrunId

                        
                    });
                    db.SaveChanges();
                }
            }
            
          
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var b = db.Urun.Where(x => x.UrunId == id).SingleOrDefault();
            if (b == null)
            {
                return HttpNotFound();
            }

            ViewBag.Kategoriler = new SelectList(db.Kategori, "KategoriId", "Adi");
            ViewBag.Markalar = new SelectList(db.Marka, "MarkaId", "Adi");
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]

        public ActionResult Edit(int id, Urun urun, HttpPostedFileBase BrandImg)
        {
            if (ModelState.IsValid)
            {
                var b = db.Urun.Where(x => x.UrunId == id).SingleOrDefault();
                if (BrandImg != null)
                {
                    //if (System.IO.File.Exists(Server.MapPath(b.ResimURL)))
                    //{
                    //    System.IO.File.Delete(Server.MapPath(b.ResimURL));
                    //}

                    WebImage img = new WebImage(BrandImg.InputStream);
                    FileInfo imginfo = new FileInfo(BrandImg.FileName);

                    string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    img.Save("~/Uploads/Urun/" + urunimgname);

                    //b.ResimURL = "/Uploads/Urun/" + urunimgname;
                }

                b.Adi = urun.Adi;
                b.Aciklama = urun.Aciklama;
                b.KategoriId = urun.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(urun);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var u = db.Urun.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }

            //if (System.IO.File.Exists(Server.MapPath(u.ResimURL)))//Buradaki hata şu 1 ürüne ait 100 foto var ama bu kontrol ettiğin hangisi ? yani bi col tip koleksiyon bu 
            //{
            //    System.IO.File.Delete(Server.MapPath(u.ResimURL));
            //}

            db.Urun.Remove(u);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
