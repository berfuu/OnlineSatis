using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using OnlineSatis.Models.DB;
using PagedList;
using PagedList.Mvc;

namespace OnlineSatis.Controllers
{
    public class HomeController : Controller
    {
        private OnlineSatisDBEntities db = new OnlineSatisDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SliderPartical()
        {
            var b_result = db.Slider.ToList();
            var result=(db.Slider.ToList().OrderByDescending(x => x.SliderId)); //en son eklenen slider ilk görülmelidir .
            return View(result);
        }
        public ActionResult Hakkimizda()
        {

            return View(db.Hakkimizda.SingleOrDefault());
        }

        public ActionResult OneCikanlar()
        {

            return View(db.OneCikanlar.ToList().OrderByDescending(x=>x.OneCikanlarId));
        }

        public ActionResult Iletisim()
        {
            return View(db.Iletisim.SingleOrDefault());
        }
        [HttpPost]
      
        public ActionResult Iletisim( string email = null, string msg = null) //bu parametreler formdan geliyor
        {

            if ( email != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com"; 
                WebMail.EnableSsl = true; //güvenli bağlantı oluşturulsun
                WebMail.UserName = "berfuceren96@gmail.com"; 
                WebMail.Password = "Ceren14545561.";
                WebMail.SmtpPort = 587;
                WebMail.Send("berfuceren96@gmail.com", null, email + "-" + msg);
                ViewBag.Uyari = "Mesajınız başarı ile gönderilmiştir.";
                Response.Redirect("/iletisim");

            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar deneyiniz";
            }
            return View();
        }

        public ActionResult Blog(int Sayfa = 1)
        {
            return View(db.Blog.Include("BlogKategori").ToList().OrderByDescending(x=>x.BlogId).ToPagedList(Sayfa, 5));
        }

        public ActionResult KategoriBlog(int id,int Sayfa=1)
        {
            var b = db.Blog.Include("BlogKategori").OrderByDescending(x=>x.BlogId).Where(x => x.BlogKategori.BlogKategoriId == id).ToPagedList(Sayfa,5);
            return View(b);
        }

        public ActionResult BlogKategoriPartical()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return PartialView(db.BlogKategori.Include("Blog").ToList().OrderByDescending(x=>x.KategoriAd));
        }

        public ActionResult BlogPartical()
        {
            return PartialView(db.Blog.ToList().OrderByDescending(x=>x.BlogId));
        }

        public ActionResult BlogDetay(int id)
        {
            var b = db.Blog.Include("BlogKategori").Include("Yorum").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }

        public JsonResult YorumYap(string cmt,string email,string name,int blogid)
        {
            if (cmt==null)
            {
                return Json(true, JsonRequestBehavior.AllowGet); //null ise aynı yerde kalsın
            }

            db.Yorum.Add(new Yorum
            {
                Icerik = cmt,
                AdSoyad = name,
                Eposta = email,
                BlogId = blogid,
                Onay=false
            });
            db.SaveChanges();   
            Response.Redirect("/Home/BlogDetay/"+blogid);
            return Json(false,JsonRequestBehavior.AllowGet); //json verilerinin alınıp gönderilmesine izin vermek.
        }

        public PartialViewResult Sepet()
        {
            return PartialView();
        }

        public ActionResult YeniUrunler(int? id)
        {
            return View(db.Urun.OrderByDescending(x=>x.EklenmeTarihi).Take(10).ToList());
        }
        public ActionResult KategoriPartical()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return PartialView(db.Kategori.Include("Urun").ToList().OrderByDescending(x => x.Adi));
        }
        public ActionResult Urun(int? id)
        {
            return View(db.Urun.OrderByDescending(x => x.UrunId).ToList());
        }

        public ActionResult UrunDetay(int id)
        {
            ViewBag.Label = new SelectList(db.UrunVaryant, "Id", "Label");
            ViewBag.Value = new SelectList(db.UrunVaryant, "Id", "Value");
            return View(db.Urun.Where(x=>x.UrunId==id).SingleOrDefault());
        }
    }
}