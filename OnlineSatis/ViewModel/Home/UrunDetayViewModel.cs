using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineSatis.Models.DB;

namespace OnlineSatis.ViewModel.Home
{
    public class UrunDetayViewModel
    {
        public UrunDetayViewModel()
        {
                urunVaryantları = new Dictionary<string, string>();
        }
        public Urun Urun { get; set; }
        public IList<Urun> urunler { get; set; }

        public IList<UrunYorum> UrunYorumlar { get; set; }



        public Dictionary<string,string> urunVaryantları { get; set; }
    }
}