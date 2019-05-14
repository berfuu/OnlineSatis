using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineSatis.Models.DB;

namespace OnlineSatis.ViewModel.Home
{
    public class IndexViewModel
    {
        public IList<Kategori> kategoriler { get; set; }

        public IList<Urun> urunler { get; set; }

        public IList<Blog> bloglar { get; set; }
        public IList<Slider> sliders { get; set; }

        public string  name { get; set; }
    }
}