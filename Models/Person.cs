using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Person")]

    // sınıftan doğrudan nesne oluşumunun önlenmesi için abstract olarak tanımlanır
    public abstract class Person
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int PersonID { get; set; }

        // uzunluğu maksimum 30 olabilir
        [StringLength(30)]
        public string PersonName { get; set; }

        [StringLength(30)]
        public string PersonSurname { get; set; }

        // sadece 11 haneli bir veri girilebilir
        [StringLength(11), MinLength(11)]
        public string PersonTel {  get; set; }

        // geçerli bir email adresi girilmesi zorunlu kılınır ve boş geçilemez
        [EmailAddress, Required]
        public string PersonMail { get; set; }

        // bir person'ın professor mü asssitant mı olduğu bilgisini tutar
        [Required]
        public string PersonType { get; set; }

        // admin paneline erişip erişemeyeceğinin belirleneceği değişken, zorunlu alan/boş geçilemez
        [Required]
        public bool IsApproved { get; set; }





        // TABLOLAR ARASI İLİŞKİLER;;;


        // address ve person tabloları arası çoka-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir person birden fazla address sahip olabilir, bir address bir person 'a ait olabilir
        public virtual List<Address> AddressList { get; set; }

    }
}