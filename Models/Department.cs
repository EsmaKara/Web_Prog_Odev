using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web_Prog_Odev.Models
{
    // veri tabanındaki tablo ismi ataması
    [Table("Department")]
    public class Department
    {
        // Primary Key olarak ayarlandı, Identity otomatik artılacak ve gerekli alan/boş geçilemez
        [Key, Required]
        public int DepartmentID { get; set; }

        // uzunluğu maksimum 60 olabilir ve zorunlu alan
        [StringLength(60), Required]
        public string DepartmentName { get; set; }

        [StringLength(400), Required]
        public string Dep_Description { get; set; }

        public int Dep_NumberOfPatients { get; set; }

        // zorunlu alan
        [Required]
        public int Dep_NumberOfBed {  get; set; }

        // yatalak hasta sayısı
        [Required]
        public int Dep_NumberOfBedridden { get; set; }

        [Required]
        public int Dep_NumberOfEmptyBed { get; set; }





        // TABLOLAR ARASI İLİŞKİLER;;;


        // emergency ve department arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir department birden fazla emergency sahip olabilir ama bir emergency bir department 'a ait olabilir
        public virtual List<Emergency> EmergencyList { get; set; }

        // professor ve department arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile)
        // bir department birden fazla professor 'e sahip olabilir ama bir professor bir department 'a ait olabilir
        public virtual List<Professor> ProfessorList { get; set; }

        // department ve shift arası bire-çok ilişki tanımlanır (virtual anahtar kelimesi ile
        // bir department birden fazla shift 'e sahip olabilir ama bir shif bir department 'a ait olabilir
        public virtual List<Shift> ShiftList { get; set; }
    }
}