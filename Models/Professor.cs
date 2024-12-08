﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Professor")]

    // Person sınıfından kalıtım alır, veritabanındaki disjoint (ayrık) ve total completeness (toplam bütünlük) ilişkisini sağlamak için 
    // Disjoint - Bir Person ya Professor olabilir ya da Assistant olabilir, iki aynı anda olamaz
    // Total Completeness - Bir Person, bir Professor ya da bir Assistant olmak zorundadır, ikisinden biri kesinlikle olmalıdır
    public class Professor:Person
    {

        [Key]
        public int PersonID { get; set; }




        // TABLOLAR ARASI İLİŞKİLER;;;


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