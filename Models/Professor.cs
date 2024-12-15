using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Professor")]

    public class Professor
    {

        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int ProfessorID { get; set; }

        // uzunluğu maksimum 30 olabilir
        [StringLength(30)]
        public string ProfName { get; set; }

        [StringLength(30)]
        public string ProfSurname { get; set; }

        // sadece 11 haneli bir veri girilebilir
        [StringLength(12), MinLength(12)]
        public string ProfTel { get; set; }

        // geçerli bir email adresi girilmesi zorunlu kılınır ve boş geçilemez
        [EmailAddress, Required]
        public string ProfMail { get; set; }








        // TABLOLAR ARASI İLİŞKİLER;;;



        // address ve person tabloları arası çoka-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir person birden fazla address sahip olabilir, bir address bir person 'a ait olabilir
        public virtual List<Address> AddressList { get; set; }

        // professor ve department arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir professor bir department 'a ait olabilir ama bir department birden fazla professor 'e sahip olabilir
        [ForeignKey("DepartmentR")]
        public int DepartmentID { get; set; }
        public virtual Department DepartmentR { get; set; }
        // değişkenin sonuna R koyulma sebebi Relationship 'leri tuttuğu anlaşılsın diye

        // available_prof ve professor arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir professor birden fazla available_prof 'a sahip olabilir, bir available_prof bir professor 'a ait olabilir
        public virtual List<Available_Prof> AvailableProfList { get; set; }
    }
}