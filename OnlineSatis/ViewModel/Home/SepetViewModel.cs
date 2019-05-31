using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineSatis.Models.DB;

namespace OnlineSatis.ViewModel.Home
{
    public class SepetViewModel
    {
        public List<SepetUrunu> SepetUrunleri { get; set; }

        public decimal SepetTutarı { get; set; }
    }

    
}