using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Address")]
    public class Address
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int AddressID { get; set; }

        [StringLength(60)]
        public string AddressDescription { get; set; }

        // uzunluğu maksimum 60 olabilir ve zorunlu alan
        [StringLength(60), Required]
        public string AddressCountry {  get; set; }    


        


        // TABLOLAR ARASI İLİŞKİLER;;;


        // address ve person tabloları arası çoka-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir address birden fazla person 'a ait olabilir, bir person birden fazla address sahip olabilir
        public virtual List<Person> Persons { get; set; }
    }
}