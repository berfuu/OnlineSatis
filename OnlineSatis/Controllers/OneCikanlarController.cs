using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class OneCikanlarController : Controller
    {
        // GET: OneCikanlar

        private OnlineSatisDBEntities db = new OnlineSatisDBEntities();
        public ActionResult Index()

        {
            return View(db.OneCikanlar.ToList());
        }

        public ActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(OneCikanlar oneCikanlar, HttpPostedFileBase ResimURL)
        {
            if (ModelState.IsValid)
            {
                if (ResimURL != null)
                {
                    WebImage img = new WebImage(ResimURL.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURL.FileName);

                    string logoname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(300, 200);
                    img.Save("~/Uploads/OneCikanlar/" + logoname);

                    oneCikanlar.ResimURL = "/Uploads/OneCikanlar/" + logoname;
                }
                db.OneCikanlar.Add(oneCikanlar);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(oneCikanlar);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.Uyari = "Güncellenecek Hizmet Bulunamadı";
            }
            var hizmet = db.OneCikanlar.Find(id);
            if (hizmet == null)
            {
                return HttpNotFound();
            }

            return View(hizmet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int? id, OneCikanlar oneCikanlar, HttpPostedFileBase ResimURL)
        {
            if (ModelState.IsValid)
            {
                var h = db.OneCikanlar.Where(x => x.OneCikanlarId == id).SingleOrDefault();
                if (ResimURL != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(h.ResimURL)))
                    {
                        System.IO.File.Delete(Server.MapPath(h.ResimURL));
                    }
                    WebImage img = new WebImage(ResimURL.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURL.FileName);

                    string hizmetname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(500, 500);
                    img.Save("~/Uploads/OneCikanlar/" + hizmetname);

                    h.ResimURL = "/Uploads/OneCikanlar/" + hizmetname;
                }

                h.Baslik = oneCikanlar.Baslik;
                h.Aciklama = oneCikanlar.Aciklama;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var h = db.OneCikanlar.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            db.OneCikanlar.Remove(h);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }


}