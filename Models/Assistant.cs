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
        [Key]
        public int PersonID { get; set; }






        // TABLOLAR ARASI İLİŞKİLER;;;


        // shift ve assistant arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir assistant birden fazla shift 'e sahip olabilir ama bir shif bir assistant 'a ait olabilir
        public virtual List<Shift> ShiftList { get; set; }

        // assistant ve appointment arası bire-bir ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir assistant birden çok appointment 'a sahip olabilir, bir appointment bir assistant 'a ait olabilir
        public virtual List<Appointment> AppointmentList { get; set; }
    }
}