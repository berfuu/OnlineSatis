using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using OnlineSatis.Models.DB;
using OnlineSatis.ViewModel.Home;
using PagedList;
using PagedList.Mvc;

namespace OnlineSatis.Controllers
{
    public class HomeController : Controller
    {
        private OnlineSatisDBEntities db = new OnlineSatisDBEntities();

        public const string CartSessionKey = "CartId";
        public string ShoppingCartId { get; set; }

        // GET: Home
        public ActionResult UyeOl()
        {

            return View();
        }

        [HttpPost]

        public ActionResult UyeOl(Musteri musteri)
        {

            musteri.Sifre = Crypto.Hash(musteri.Sifre, "MD5");
            db.Musteri.Add(musteri);
            db.SaveChanges();
            Session["musteriid"] = musteri.MusteriId;
            Session["email"] = musteri.Email;
            return RedirectToAction("Login");

        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Musteri musteri)
        {

            var login = db.Musteri.Where(x => x.Email == musteri.Email).SingleOrDefault();
            if (login.Email == musteri.Email && login.Sifre == Crypto.Hash(musteri.Sifre, "MD5"))
            {
                Session["musteriid"] = login.MusteriId;
                Session["email"] = login.Email;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Uyari = "Kullanıcı adı yada şifre yanlış";
            return View(musteri);

        }

        public ActionResult Logout()
        {
            Session["musteriid"] = null;
            Session["email"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string email)
        {
            var mail = db.Musteri.Where(x => x.Email == email).SingleOrDefault();
            if (mail != null)
            {
                Random rnd = new Random();
                int yenisifre = rnd.Next();

                Musteri musteri = new Musteri();
                mail.Sifre = Crypto.Hash(Convert.ToString(yenisifre), "MD5");
                db.SaveChanges();

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "berfuceren96@gmail.com";
                WebMail.Password = "Ceren14545561.";
                WebMail.SmtpPort = 587;
                WebMail.Send(email, " Giriş Şifreniz", "Şifreniz :" + yenisifre);
                ViewBag.Uyari = "Şifreniz Başarı ile gönderilmiştir";


            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar deneyiniz";
            }
            return View();

        }

        public ActionResult Index()
        {
            ViewBag.SepetMiktar = GetCount();
            var model = new IndexViewModel();
            model.kategoriler = db.Kategori.ToList();
            model.urunler = db.Urun.Take(4).ToList();
            model.bloglar = db.Blog.Take(3).ToList();
            var b_result = db.Slider.ToList();
            model.sliders = db.Slider.ToList().OrderByDescending(x => x.SliderId)
                .ToList();
            ; return View(model);
        }


        public ActionResult SliderPartical()
        {
            //en son eklenen slider ilk görülmelidir .
            return View();
        }
        public ActionResult Hakkimizda()
        {

            return View(db.Hakkimizda.SingleOrDefault());
        }

        public ActionResult OneCikanlar()
        {

            return View(db.OneCikanlar.ToList().OrderByDescending(x => x.OneCikanlarId));
        }

        public ActionResult Iletisim()
        {
            return View(db.Iletisim.SingleOrDefault());
        }
        [HttpPost]

        public ActionResult Iletisim(string email = null, string msg = null) //bu parametreler formdan geliyor
        {

            if (email != null)
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
            return View(db.Blog.Include("BlogKategori").ToList().OrderByDescending(x => x.BlogId).ToPagedList(Sayfa, 5));
        }

        public ActionResult KategoriBlog(int id, int Sayfa = 1)
        {
            var b = db.Blog.Include("BlogKategori").OrderByDescending(x => x.BlogId).Where(x => x.BlogKategori.BlogKategoriId == id).ToPagedList(Sayfa, 5);
            return View(b);
        }

        public ActionResult BlogKategoriPartical()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return PartialView(db.BlogKategori.Include("Blog").ToList().OrderByDescending(x => x.KategoriAd));
        }

        public ActionResult BlogPartical()
        {
            return PartialView(db.Blog.ToList().OrderByDescending(x => x.BlogId));
        }
        public PartialViewResult Sepet()
        {
            //test purpose
            // Session[CartSessionKey] = "a659275b-973b-4aff-9787-6a5d47781a8d";

            var tempCart = new SepetViewModel();
            tempCart.SepetUrunleri = GetCartItems();
            tempCart.SepetTutarı = GetTotal();
            return PartialView(tempCart);
        }

        public ActionResult BlogDetay(int id)
        {
            var b = db.Blog.Include("BlogKategori").Include("Yorum").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }

        public JsonResult YorumYap(string cmt, string email, string name, int blogid)
        {
            if (cmt == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet); //null ise aynı yerde kalsın
            }

            db.Yorum.Add(new Yorum
            {
                Icerik = cmt,
                AdSoyad = name,
                Eposta = email,
                BlogId = blogid,
                Onay = false
            });
            db.SaveChanges();
            Response.Redirect("/Home/BlogDetay/" + blogid);
            return Json(false, JsonRequestBehavior.AllowGet); //json verilerinin alınıp gönderilmesine izin vermek.
        }

        public ActionResult YeniUrunler(int? id)
        {
            return View(db.Urun.OrderByDescending(x => x.EklenmeTarihi).Take(10).ToList());
        }
        public ActionResult KategoriPartical()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return PartialView(db.Kategori.Include("Urun").ToList().OrderByDescending(x => x.Adi));
        }
        public ActionResult Urun(int? id)
        {
            var a = db.Urun.OrderByDescending(x => x.UrunId).ToList();
            return View(a);
        }

        public ActionResult KategoriUrun(int id)
        {
            var u = db.Urun.Include("Kategori").OrderByDescending(x => x.UrunId).Where(x => x.Kategori.KategoriId == id).ToList();
            return View(u);
        }
        public ActionResult ShoppingCart()
        {
            SepetViewModel vm = new SepetViewModel();
            vm.SepetUrunleri = GetCartItems();
            return View(vm);
        }

        public ActionResult UrunDetay(int id)
        {
            UrunDetayViewModel urunDetay = new UrunDetayViewModel();

            urunDetay.Urun = db.Urun.Where(x => x.UrunId == id).SingleOrDefault();
            urunDetay.UrunYorumlar = db.UrunYorum.Where(x => x.UrunId == id).ToList();
            var variantsFromDb = db.UrunVaryant.Where(u => u.UrunId == id).ToList();

            foreach (var item in variantsFromDb)
            {
                urunDetay.urunVaryantları.Add(item.Label, item.Value);
            }
            urunDetay.urunler = db.Urun.Take(4).ToList();
            return View(urunDetay);
        }

        [HttpPost]
        public ActionResult UrunYorumEkle(int UrunId, string review, string name, string email)
        {
            var yeniYourm = new UrunYorum()
            {
                AdSoyad = name,
                Eposta = email,
                Icerik = review,
                UrunId = UrunId
            };
            db.UrunYorum.Add(yeniYourm);
            db.SaveChanges();
            return RedirectToAction(nameof(UrunDetay), new { id = UrunId });
        }
        [HttpPost]
        public ActionResult urunAra(string search)
        {
            var urunler = db.Urun.Where(u => u.Adi.Contains(search) || u.Aciklama.Contains(search)).ToList();
            return View("Urun", urunler);
        }

        [HttpPost]
        public ActionResult UrunKaldır(int urunId, string returnUrl = "")
        {
            UrunKaldır(urunId);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json("Cart item removed!", MediaTypeNames.Text.Plain);
            //if (!String.IsNullOrWhiteSpace(returnUrl))
            //    return Redirect(returnUrl);
            //return RedirectToAction(nameof(ShoppingCart));
        }
        private void UrunKaldır(int urunId)
        {
            var cartId = GetCartId();
            RemoveItem(cartId, urunId);
            //if (!String.IsNullOrWhiteSpace(returnUrl))
            //    return Redirect(returnUrl);
            //return RedirectToAction(nameof(ShoppingCart));
        }

        [HttpPost]
        public ActionResult SepetGuncelle(FormCollection formItems)
        {
            var cartId = GetCartId();
            //UpdateShoppingCartDatabase(cartId, urunId);
            //if (!String.IsNullOrWhiteSpace(returnUrl))
            //    return Redirect(returnUrl);
            return RedirectToAction(nameof(ShoppingCart));
        }
        [HttpPost]
        public ActionResult UrunMiktarArttir(int urunId)
        {
            var cartId = GetCartId();
            var itemToUpdate = new ShoppingCartUpdates() { UrunId = urunId, UrunArttır = true };
            UpdateShoppingCartItemDatabase(cartId, itemToUpdate);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json("Cart item updated!", MediaTypeNames.Text.Plain);
        }
        [HttpPost]
        public ActionResult UrunMiktarAzalt(int urunId)
        {
            var cartId = GetCartId();
            var itemToUpdate = new ShoppingCartUpdates() { UrunId = urunId, UrunArttır = false };
            UpdateShoppingCartItemDatabase(cartId, itemToUpdate);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json("Cart item updated!", MediaTypeNames.Text.Plain);
        }

        [HttpPost]
        public ActionResult AddToCart(int UrunId, int miktar = 1)
        {
            ShoppingCartId = GetCartId();

            var cartItem = db.SepetUrunu.SingleOrDefault(
                c => c.SeptId == ShoppingCartId
                     && c.UrunId == UrunId);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new SepetUrunu()
                {
                    SepetUrunId = Guid.NewGuid().ToString(),
                    UrunId = UrunId,
                    SeptId = ShoppingCartId,
                    Urun = db.Urun.SingleOrDefault(
                        p => p.UrunId == UrunId),
                    Miktar = miktar,
                    OluşturmaTarihi = DateTime.Now
                };

                db.SepetUrunu.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Miktar++;
            }
            db.SaveChanges();
            return RedirectToAction(nameof(UrunDetay), new { id = UrunId });
        }


        private List<SepetUrunu> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            return db.SepetUrunu.Where(
                c => c.SeptId == ShoppingCartId).ToList();
        }

        private decimal GetTotal()
        {
            ShoppingCartId = GetCartId();

            decimal? total = decimal.Zero;

            total = (decimal?)db.SepetUrunu.Where(s => s.SeptId == ShoppingCartId)
                .Select(s => (int?)s.Miktar * s.Urun.SatisFiyat).Sum();

            //total = (decimal?)(from cartItems in _db.ShoppingCartItems
            //    where cartItems.CartId == ShoppingCartId
            //    select (int?)cartItems.Quantity *
            //           cartItems.Product.UnitPrice).Sum();

            return total ?? decimal.Zero;
        }

        private void RemoveItem(string removeCartID, int removeProductID)
        {
            try
            {
                var myItem = db.SepetUrunu.Where(s => s.SeptId == removeCartID && s.UrunId == removeProductID)
                    .SingleOrDefault();
                //var myItem1 = (from c in _db.ShoppingCartItems where c.CartId == removeCartID && c.Product.ProductID == removeProductID select c).FirstOrDefault();
                if (myItem != null)
                {
                    // Remove Item.
                    db.SepetUrunu.Remove(myItem);
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
            }
        }
        private void UpdateShoppingCartItemDatabase(String cartId, ShoppingCartUpdates CartItemUpdate)
        {
            try
            {
                var urun = db.SepetUrunu.Where(x => x.SeptId == cartId && x.UrunId == CartItemUpdate.UrunId).SingleOrDefault();
                var yeniMiktar = CartItemUpdate.UrunArttır ? urun.Miktar.Value + 1 : urun.Miktar.Value - 1;
                if (yeniMiktar < 1)
                    UrunKaldır(CartItemUpdate.UrunId);
                else
                    UpdateItem(cartId, urun.UrunId.Value, yeniMiktar);
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Item Database - " + exp.Message.ToString(), exp);
            }
        }
        public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] CartItemUpdates)
        {
            try
            {
                int CartItemCount = CartItemUpdates.Count();
                List<SepetUrunu> myCart = GetCartItems();
                foreach (var cartItem in myCart)
                {
                    // Iterate through all rows within shopping cart list
                    for (int i = 0; i < CartItemCount; i++)
                    {
                        if (cartItem.Urun.UrunId == CartItemUpdates[i].UrunId)
                        {
                            if (CartItemUpdates[i].Miktar < 1 || CartItemUpdates[i].UrunuKaldır == true)
                            {
                                RemoveItem(cartId, cartItem.UrunId.Value);
                            }
                            else
                            {
                                UpdateItem(cartId, cartItem.UrunId.Value, CartItemUpdates[i].Miktar);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            try
            {

                var myItem = (from s in db.SepetUrunu where s.SeptId == updateCartID && s.Urun.UrunId == updateProductID select s).FirstOrDefault();
                if (myItem != null)
                {
                    myItem.Miktar = quantity;
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
            }
        }

        private void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = db.SepetUrunu.Where(
                c => c.SeptId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                db.SepetUrunu.Remove(cartItem);
            }
            // Save changes.             
            db.SaveChanges();
        }

        private int GetCount()
        {
            ShoppingCartId = GetCartId();

            // Get the count of each item in the cart and sum them up 
            int? count = db.SepetUrunu.Where(s => s.SeptId == ShoppingCartId).Select(s => s.Miktar).Sum();
            //int? count1 = (from cartItems in _db.ShoppingCartItems
            //              where cartItems.CartId == ShoppingCartId
            //              select (int?)cartItems.Quantity).Sum();
            // Return 0 if all entries are null         
            return count ?? 0;
        }


        private string GetCartId()
        {
            if (Session[CartSessionKey] == null)
            {

                // Generate a new random GUID using System.Guid class.     
                Guid tempCartId = Guid.NewGuid();
                Session[CartSessionKey] = tempCartId.ToString();
            }
            return Session[CartSessionKey].ToString();
        }

        public struct ShoppingCartUpdates
        {
            public int UrunId;
            public int Miktar;
            public bool UrunuKaldır;
            public bool UrunArttır;
        }
    }
}