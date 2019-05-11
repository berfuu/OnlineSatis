using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineSatis.Models.DB;

namespace OnlineSatis.Controllers
{
    public class GaleriController : Controller
    {
        private OnlineSatisDBEntities db = null;

        public GaleriController()
        {
            if (db == null)
            {
                db = new OnlineSatisDBEntities();
            }
        }
        // GET: Galeri
        /// <summary>
        /// Ürüne ait resim galerisi
        /// </summary>
        /// <param name="urunid">Ürün Id'si</param>
        /// <returns>List Of Resim</returns>
        public ActionResult Index(int ?urunid)
        {
            if (urunid == null) return null;
            var resultImages = db.Resim.Where(sa => sa.UrunId == urunid)
                .ToList();
            return View(resultImages);
        }

        public ActionResult Delete(int resimid, int? urunid)
        {
            var finded = db.Resim.Find(resimid);//Siz de uyuyonuz valla ha sizi deniyorum : hayır kabul etmiyorum burda söyledim ben ama sana değil:))
            if (finded == null) return null;
            //Silindiği taktirde aynı galeriye dönebilsin.
            //Sizi deniyorum ben :D bak mesela silmemişiz resmi ? kimse uyarmadı
            //biz bunu gördük ama sen bir şeyler yapıyorsun diye araya giremedik remove yapmadık :)))
            db.Resim.Remove(finded);
            db.SaveChanges();
            return RedirectToAction("Index", "Galeri", new {urunid = urunid});
        }
    }
}