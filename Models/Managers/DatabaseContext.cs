using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Entity Framework kullanabilmek için ad alanı eklenir
using System.Data.Entity;
using System.EnterpriseServices;
using System.Security.Cryptography;
using System.Net;

namespace Web_Prog_Odev.Models.Managers
{
    public class DatabaseContext: DbContext
    {
        // tasarlanan sınıflar için veri tabanındaki tabloyu tutacak değişkenler oluşturuluyor;
        // indert, update, delete işlemleri için bu değişkenler kullanılır
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Available_Prof> AvailableProfs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }

        //// constructer metoduna DatabaseCreator sınıfı eklenir ki çalışsın ve veri tabanı yoksa oluşturulsun  / ama artık migration kullanılıyor
        //public DatabaseContext() : base("WProgOdev_Database")
        //{ 
        //    Database.SetInitializer(new DatabaseCreator());
        //}




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Available_Prof>()
                .HasRequired(ap => ap.AppointmentR) // İlişki türünü belirtir (Required, Optional)
                .WithRequiredPrincipal(a => a.AvailableProfR); // Diğer tarafın navigasyon özelliği
        }

    }







    // Sorunlarla karşılaştığım için DatabaseCreator yerine Entity Framework Code-First Migrations kullanmaya karar verdim



    //// Bu sınıf, veritabanı varlığını kontrol eder ve eğer veritabanı yoksa sınıf tanımlamalarına göre yeni bir veritabanı oluşturur (generic).
    //public class DatabaseCreator : CreateDatabaseIfNotExists<DatabaseContext>
    //{
    //    private int NumberOfProfessor = 3;
    //    private int NumberOfAssistant = 15;
    //    public override void InitializeDatabase(DatabaseContext context)
    //    {
    //        base.InitializeDatabase(context);
    //    }

    //    // database oluşturulduktan sonra çalışır, veri ekleme işlemleri burada yapılır
    //    // Veriler için rastgele veriler üretebilen FakeData NuGet paketi kullanılır
    //    protected override void Seed(DatabaseContext context)
    //    {

    //        Console.WriteLine("Seed metodu çağrıldı."); // Debug için ekleme (kontrol)

    //        // Department için verileri ekleme
    //        string[] departmentNameArray = new string[3]
    //        {
    //            "Pediatric Emergency",
    //            "Pediatric Intensive Care Unit (PICU)",
    //            "Pediatric Hematology and Oncology"
    //        };
    //        for (int i = 0; i < departmentNameArray.Length; i++)
    //        {
    //            Department department = new Department();
    //            department.DepartmentID = i;
    //            department.DepartmentName = departmentNameArray[i];
    //            department.Dep_Description = " !? ";
    //            department.Dep_NumberOfPatients = FakeData.NumberData.GetNumber(100);
    //            department.Dep_NumberOfBed = FakeData.NumberData.GetNumber(50);

    //            int randomNumber = FakeData.NumberData.GetNumber(50);
    //            if (department.Dep_NumberOfBed >= randomNumber)
    //                department.Dep_NumberOfBedridden = randomNumber;

    //            department.Dep_NumberOfEmptyBed = department.Dep_NumberOfBed - department.Dep_NumberOfBedridden;

    //            context.Departments.Add(department);
    //        }
    //        // eklemek için kodlar yazıldı ancak tablolara eklenebilmesi için değişikliklerin kaydedilmesi gerekir
    //        context.SaveChanges();


    //        // Person verileri eklenecek, Professor ve Assistant olarak;
            
    //        List<Department> deps = context.Departments.ToList();
    //        int pSayac = 1;
    //        // Professor için verileri ekleme ve Department ile professor'lerin bağlanması
    //        foreach (Department d in deps) {
    //            for (int i = 0; i <= NumberOfProfessor; i++)
    //            {
    //                Professor professor = new Professor();
    //                professor.ProfessorID = pSayac;
    //                professor.ProfName = FakeData.NameData.GetFirstName();
    //                professor.ProfSurname = FakeData.NameData.GetSurname();
    //                professor.ProfTel = FakeData.PhoneNumberData.GetPhoneNumber();
    //                professor.ProfMail = FakeData.NetworkData.GetEmail();

    //                professor.DepartmentR = d;

    //                context.Professors.Add(professor);
    //                pSayac += 1;
    //            }
    //        }
    //        context.SaveChanges();


    //        // Assistant için verileri ekleme
    //        for (int i = 0; i <= NumberOfAssistant; i++)
    //        {
    //            Assistant assistant = new Assistant();
    //            assistant.AssistantID = i + 1;
    //            assistant.AssistName = FakeData.NameData.GetFirstName();
    //            assistant.AssistSurname = FakeData.NameData.GetSurname();
    //            assistant.AssistTel = FakeData.PhoneNumberData.GetPhoneNumber();
    //            assistant.AssistMail = FakeData.NetworkData.GetEmail();

    //            context.Assistants.Add(assistant);
    //        }
    //        context.SaveChanges();


    //        // veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
    //        List<Professor> profs = context.Professors.ToList();
    //        // Address için verileri ekleme ve Person ile bağlantısı
    //        foreach (Professor p in profs)
    //        {
    //            Address address = new Address();
    //            address.AddressCountry = FakeData.PlaceData.GetCountry();
    //            address.AddressDescription = FakeData.PlaceData.GetAddress();

    //            address.ProfessorID =p.ProfessorID;
    //            address.ProfessorR = p;

    //            context.Addresses.Add(address);
    //        }
    //        context.SaveChanges();
    //        // veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
    //        List<Assistant> assists = context.Assistants.ToList();
    //        // Address için verileri ekleme ve Person ile bağlantısı
    //        foreach (Assistant a in assists)
    //        {
    //            Address address = new Address();
    //            address.AddressCountry = FakeData.PlaceData.GetCountry();
    //            address.AddressDescription = FakeData.PlaceData.GetAddress();

    //            address.AssistantID = a.AssistantID;


    //            context.Addresses.Add(address);
    //        }
    //        context.SaveChanges();



    //        // Bütün tablolara en az bir veri eklenmesi gerekiyor;;;

    //        {
    //            // Shift için birkaç veri ekleyelim
    //            Shift shift1 = new Shift { ShiftStart = DateTime.Now, ShiftEnd = DateTime.Now.AddDays(1) };
    //            shift1.AssistantID = 2;
    //            shift1.DepartmentID = 3;
    //            context.Shifts.Add(shift1);
    //        }
    //        context.SaveChanges();


    //        base.Seed(context);
    //    }
    //}
        
}