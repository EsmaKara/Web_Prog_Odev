using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Shift")]
    public class Shift
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int ShiftID { get; set; }

        // zorunlu alan
        [Required]
        public DateTime ShiftStart { get; set; }

        [Required]
        public DateTime ShiftEnd { get; set; }




        // TABLOLAR ARASI İLİŞKİLER;;;


        // shift ve assistant arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir shif bir assistant 'a ait olabilir ama bir assistant birden fazla shift 'e sahip olabilir
        [ForeignKey("AssistantR")]
        public int PersonID { get; set; }
        public virtual Assistant AssistantR { get; set; }

        // department ve shift arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile
        // bir shif bir department 'a ait olabilir ama bir department birden fazla shift 'e sahip olabilir
        [ForeignKey("DepartmentR")]
        public int DepartmentID { get; set; }
        public virtual Department DepartmentR { get; set; }
        // değişkenin sonuna R koyulma sebebi Relationship 'leri tuttuğu anlaşılsın diye
    }
}