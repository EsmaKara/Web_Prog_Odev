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

    public class Assistant
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int AssistantID { get; set; }

        // uzunluğu maksimum 30 olabilir
        [StringLength(30)]
        public string AssistName { get; set; }

        [StringLength(30)]
        public string AssistSurname { get; set; }

        // sadece 11 haneli bir veri girilebilir
        [StringLength(12), MinLength(12)]
        public string AssistTel { get; set; }

        // geçerli bir email adresi girilmesi zorunlu kılınır ve boş geçilemez
        [EmailAddress, Required]
        public string AssistMail { get; set; }




        // TABLOLAR ARASI İLİŞKİLER;;;


        // address ve person tabloları arası çoka-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir person birden fazla address sahip olabilir, bir address bir person 'a ait olabilir
        public virtual List<Address> AddressList { get; set; }

        // shift ve assistant arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir assistant birden fazla shift 'e sahip olabilir ama bir shif bir assistant 'a ait olabilir
        public virtual List<Shift> ShiftList { get; set; }

        // assistant ve appointment arası bire-bir ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir assistant birden çok appointment 'a sahip olabilir, bir appointment bir assistant 'a ait olabilir
        public virtual List<Appointment> AppointmentList { get; set; }
    }
}