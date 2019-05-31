using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
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

            ViewBag.Kategoriler = new SelectList(db.Kategori, "KategoriId", "Adi"); // contoller den view veri taşımak için kullanılır.
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

            if (BrandImg != null)
            {
                foreach (var item in BrandImg)
                {
                    WebImage img = new WebImage(item.InputStream);
                    FileInfo imginfo = new FileInfo(item.FileName);

                    string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    string imgPath = "~/Uploads/Urun/" + urunimgname;
                    img.Save(imgPath);
                    db.Resim.Add(new Resim()
                    {
                        ResimURL = imgPath.Remove(0, 1),
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

        public ActionResult Edit(Urun urun, List<HttpPostedFileBase> BrandImg)
        {

            if (ModelState.IsValid)
            {
                var b = db.Urun.Where(x => x.UrunId == urun.UrunId).SingleOrDefault();
                if (BrandImg != null)
                {
                    foreach (var item in BrandImg)
                    {
                        WebImage img = new WebImage(item.InputStream);
                        FileInfo imginfo = new FileInfo(item.FileName);

                        string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                        img.Resize(600, 400);
                        string imgPath = "/Uploads/Urun/" + urunimgname;
                        img.Save(imgPath);

                        db.SaveChanges();
                    }
                }

                b.Adi = urun.Adi;
                b.Aciklama = urun.Aciklama;
                b.Kategori = urun.Kategori;
                b.AlisFiyat = urun.AlisFiyat;
                b.SatisFiyat = urun.SatisFiyat;
                b.Resim = urun.Resim;
                b.Marka = urun.Marka;
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            return View(urun);
        }

        [HttpPost]
        [Route("ResimEkle/{urunId}")]
        public ActionResult ResimEkle(int urunId, List<HttpPostedFileBase> BrandImg)
        {

            var b = db.Urun.Where(x => x.UrunId == urunId).SingleOrDefault();
            if (BrandImg != null)
            {
                foreach (var item in BrandImg)
                {
                    WebImage img = new WebImage(item.InputStream);
                    FileInfo imginfo = new FileInfo(item.FileName);

                    string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    string imgPath = "~/Uploads/Urun/" + urunimgname;
                    img.Save(imgPath);
                    db.Resim.Add(new Resim()
                    {
                        ResimURL = imgPath.Remove(0, 1),
                        UrunId = b.UrunId
                    });
                    db.SaveChanges();
                }
               
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json("Images added!", MediaTypeNames.Text.Plain);
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
