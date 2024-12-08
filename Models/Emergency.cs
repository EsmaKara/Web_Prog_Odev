using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Emergency")]
    public class Emergency
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int EmergencyID { get; set; }

        // uzunluğu maksimum 200 karakter olabilir ve zorunlu alan
        [StringLength(100), Required]
        public string EmergencyName { get; set; }

        [StringLength(200)]
        public string EmergencyDescription { get; set; }




        // TABLOLAR ARASI İLİŞKİLER;;;


        // emergency ve department arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir emergency bir department 'a ait olabilir ama bir department birden fazla emergency sahip olabilir
        [ForeignKey("DepartmentR")]
        public int DepartmentID { get; set; }
        public virtual Department DepartmentR { get; set; }
        // değişkenin sonuna R koyulma sebebi Relationship 'leri tuttuğu anlaşılsın diye
    }
}