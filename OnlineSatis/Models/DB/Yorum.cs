//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineSatis.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Yorum
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Yorum()
        {
            this.OneCikanlar = new HashSet<OneCikanlar>();
        }
    
        public int YorumId { get; set; }
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        public string Icerik { get; set; }
        public int BlogId { get; set; }
        public bool Onay { get; set; }
    
        public virtual Blog Blog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OneCikanlar> OneCikanlar { get; set; }
    }
}
