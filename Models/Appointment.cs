using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Appointment")]
    public class Appointment
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int AppointmentID { get; set; }



        // TABLOLAR ARASI İLİŞKİLER;;;


        // available_prof ve appointment arası bire-bir ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir appointment bir available_prof 'a ait olabilir, bir available_prof bir appointment 'a sahip olabilir

        // List olmasa bile ForeignKey tanımlaması yok çünkü bağımlı tablo appointment'tır ve FK tanımlaması Available_Prof'ta yapılır !
        public virtual Available_Prof AvailableProfR { get; set; }

        // assistant ve appointment arası bire-bir ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir appointment bir assistant 'a ait olabilir, bir assistant birden fazla appointment 'a sahip olabilir
        [ForeignKey("AssistantR")]
        public int AssistantID {  get; set; }
        public virtual Assistant AssistantR { get; set; }
        // değişkenin sonuna R koyulma sebebi Relationship 'leri tuttuğu anlaşılsın diye
    }
}