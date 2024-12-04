using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Assistant")]

    // Person sınıfından kalıtım alır, veritabanındaki disjoint (ayrık) ve total completeness (toplam bütünlük) ilişkisini sağlamak için 
    // Disjoint - Bir Person ya Professor olabilir ya da Assistant olabilir, iki aynı anda olamaz
    // Total Completeness - Bir Person, bir Professor ya da bir Assistant olmak zorundadır, ikisinden biri kesinlikle olmalıdır
    public class Assistant:Person
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int AssistantID { get; set; }


        // admin paneline erişip erişemeyeceğinin belirleneceği değişken, zorunlu alan/boş geçilemez
        [Required]
        public bool IsApproved { get; set; }
    }
}