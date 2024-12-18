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
            // Bire bir ilişki tanımlayabilmek adına
            modelBuilder.Entity<Available_Prof>()
                .HasOptional(ap => ap.AppointmentR) // Available_Prof bir Appointment ile ilişkili olabilir (Opsiyonel)
                .WithRequired(a => a.AvailableProfR) // Appointment, Available_Prof ile ilişkili olmak zorunda
                .WillCascadeOnDelete(true); // Cascade silme davranışı







            // gerekli FK ilişkileri çünkü silme durumlarında cascade belirlenmeli

            modelBuilder.Entity<Assistant>()
                .HasMany(a => a.AppointmentList) // Bir Assistant'ın birden fazla Appointment'ı olabilir
                .WithRequired(ap => ap.AssistantR) // Her Appointment bir Assistant ile ilişkilidir
                .HasForeignKey(ap => ap.AssistantID) // Foreign Key
                .WillCascadeOnDelete(true); // Assistant silinince, bağlı Appointment'lar da silinir

            modelBuilder.Entity<Emergency>()
                .HasRequired(e => e.DepartmentR) // Her Emergency bir Department ile ilişkilidir
                .WithMany(d => d.EmergencyList) // Bir Department birçok Emergency'ye sahip olabilir
                .HasForeignKey(e => e.DepartmentID) // Foreign Key
                .WillCascadeOnDelete(true); // Department silindiğinde Emergency'ler de silinir

            modelBuilder.Entity<Professor>()
                .HasRequired(p => p.DepartmentR) // Her Professor bir Department ile ilişkilidir
                .WithMany(d => d.ProfessorList) // Bir Department birçok Professor'e sahip olabilir
                .HasForeignKey(p => p.DepartmentID) // Foreign Key
                .WillCascadeOnDelete(true); // Department silindiğinde bağlı Professors silinir

            modelBuilder.Entity<Available_Prof>()
                .HasRequired(ap => ap.ProfessorR) // Her Available_Prof bir Professor ile ilişkilidir
                .WithMany(p => p.AvailableProfList) // Bir Professor birçok Available_Prof'a sahip olabilir
                .HasForeignKey(ap => ap.ProfessorID) // Foreign Key
                .WillCascadeOnDelete(true); // Professor silindiğinde bağlı Available_Prof kayıtları silinir

            modelBuilder.Entity<Address>()
                .HasOptional(a => a.ProfessorR) // Address, Professor ile opsiyonel ilişkilidir
                .WithMany(p => p.AddressList)  // Bir Professor birden fazla Address'e sahip olabilir
                .HasForeignKey(a => a.ProfessorID) // Foreign Key
                .WillCascadeOnDelete(true); // Professor silinince bağlı Address kayıtları silinir

            modelBuilder.Entity<Address>()
                .HasOptional(a => a.AssistantR) // Address, Assistant ile opsiyonel ilişkilidir
                .WithMany(p => p.AddressList)   // Bir Assistant birden fazla Address'e sahip olabilir
                .HasForeignKey(a => a.AssistantID)  // Foreign Key
                .WillCascadeOnDelete(true);    // Assistant silinince bağlı Address kayıtları silinir

            modelBuilder.Entity<Shift>()
                .HasRequired(s => s.AssistantR) // Shift bir Assistant ile ilişkilidir (zorunlu)
                .WithMany(a => a.ShiftList)    // Bir Assistant birden fazla Shift'e sahip olabilir
                .HasForeignKey(s => s.AssistantID) // Foreign Key tanımlaması
                .WillCascadeOnDelete(true);    // Assistant silindiğinde Shiftler de silinir

            modelBuilder.Entity<Department>()
                .HasMany(d => d.ShiftList)      // Bir Department'ın birden fazla Shit'i olabilir
                .WithRequired(s => s.DepartmentR)       // Her Shift bir Department ile ilişkilidir
                .HasForeignKey(s => s.DepartmentID)     // Foreign Key
                .WillCascadeOnDelete(true); // Department silinince Shifts silinir

        }

    }







    // Sorunlarla karşılaştığım için DatabaseCreator yerine Entity Framework Code-First Migrations kullanmaya karar verdim



    //// Bu sınıf, veritabanı varlığını kontrol eder ve eğer veritabanı yoksa sınıf tanımlamalarına göre yeni bir veritabanı oluşturur (generic).
    //public class DatabaseCreator : CreateDatabaseIfNotExists<DatabaseContext>
    //{
    //      int NumberOfProfessor = 3;
            //int NumberOfAssistant = 15;

            ////database oluşturulduktan sonra çalışır, veri ekleme işlemleri burada yapılır
            //// Veriler için rastgele veriler üretebilen FakeData NuGet paketi kullanılır

            //Console.WriteLine("Seed metodu çağrıldı."); // Debug için ekleme (kontrol)

            //// Department için verileri ekleme
            //string[] departmentNameArray = new string[3]
            //{
            //            "Pediatric Emergency",
            //            "Pediatric Intensive Care Unit (PICU)",
            //            "Pediatric Hematology and Oncology"
            //};
            //for (int i = 0; i < departmentNameArray.Length; i++)
            //{
            //    Department department = new Department();
            //    department.DepartmentID = i;
            //    department.DepartmentName = departmentNameArray[i];
            //    department.Dep_Description = " !? ";
            //    department.Dep_NumberOfPatients = FakeData.NumberData.GetNumber(100);
            //    department.Dep_NumberOfBed = FakeData.NumberData.GetNumber(50);

            //    int randomNumber = FakeData.NumberData.GetNumber(50);
            //    if (department.Dep_NumberOfBed >= randomNumber)
            //        department.Dep_NumberOfBedridden = randomNumber;

            //    department.Dep_NumberOfEmptyBed = department.Dep_NumberOfBed - department.Dep_NumberOfBedridden;

            //    context.Departments.Add(department);
            //}
            //// eklemek için kodlar yazıldı ancak tablolara eklenebilmesi için değişikliklerin kaydedilmesi gerekir
            //context.SaveChanges();


            //// Professor ve Assistant verileri eklenecek

            //List<Department> deps = context.Departments.ToList();
            //int pSayac = 1;
            //// Professor için verileri ekleme ve Department ile professor'lerin bağlanması
            //foreach (Department d in deps)
            //{
            //    for (int i = 0; i <= NumberOfProfessor; i++)
            //    {
            //        Professor professor = new Professor();
            //        professor.ProfName = FakeData.NameData.GetFirstName();
            //        professor.ProfSurname = FakeData.NameData.GetSurname();
            //        professor.ProfTel = FakeData.PhoneNumberData.GetPhoneNumber();
            //        professor.ProfMail = FakeData.NetworkData.GetEmail();

            //        professor.DepartmentR = d;

            //        context.Professors.Add(professor);
            //        pSayac += 1;
            //    }
            //}
            //context.SaveChanges();


            //// Assistant için verileri ekleme
            //for (int i = 0; i <= NumberOfAssistant; i++)
            //{
            //    Assistant assistant = new Assistant();
            //    assistant.AssistName = FakeData.NameData.GetFirstName();
            //    assistant.AssistSurname = FakeData.NameData.GetSurname();
            //    assistant.AssistTel = FakeData.PhoneNumberData.GetPhoneNumber();
            //    assistant.AssistMail = FakeData.NetworkData.GetEmail();

            //    context.Assistants.Add(assistant);
            //}
            //context.SaveChanges();



            //// veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
            //List<Professor> profs = context.Professors.ToList();
            //// Address için verileri ekleme ve Person ile bağlantısı
            //foreach (Professor p in profs)
            //{
            //    Address address = new Address();
            //    address.AddressCountry = FakeData.PlaceData.GetCountry();
            //    address.AddressDescription = FakeData.PlaceData.GetAddress();

            //    address.ProfessorID = p.ProfessorID;
            //    address.ProfessorR = p;

            //    context.Addresses.Add(address);
            //}
            //context.SaveChanges();
            //// veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
            //List<Assistant> assists = context.Assistants.ToList();
            //// Address için verileri ekleme ve Person ile bağlantısı
            //foreach (Assistant a in assists)
            //{
            //    Address address = new Address();
            //    address.AddressCountry = FakeData.PlaceData.GetCountry();
            //    address.AddressDescription = FakeData.PlaceData.GetAddress();

            //    address.AssistantID = a.AssistantID;


            //    context.Addresses.Add(address);
            //}
            //context.SaveChanges();



            //// Bütün tablolara en az bir veri eklenmesi gerekiyor;;;

            //{
            //    // Shift için bir veri ekleyelim
            //    Shift shift1 = new Shift { ShiftStart = DateTime.Now, ShiftEnd = DateTime.Now.AddDays(1) };
            //    shift1.AssistantID = context.Assistants.ToList().FirstOrDefault().AssistantID;
            //    shift1.DepartmentID = context.Departments.ToList().FirstOrDefault().DepartmentID; ;
            //    context.Shifts.Add(shift1);
            //}
            //context.SaveChanges();

            //{
            //    // Emergency için bir veri ekleyelim
            //    Emergency emergency = new Emergency { EmergencyName = "Help", EmergencyDescription = "Extra help needed!", EmergencyDate = DateTime.Now };
            //    emergency.DepartmentID = context.Departments.ToList().FirstOrDefault().DepartmentID; ;
            //    context.Emergencies.Add(emergency);
            //}
            //context.SaveChanges();

            //{
            //    // Available_Prof için birkaç veri ekleyelim, biri için randevu alınmış olsun
            //    Available_Prof available_Prof1 = new Available_Prof { AvailableProfDateStart = DateTime.Now, AvailableProfDateEnd = DateTime.Now.AddMinutes(120), IsAvailable = false };
            //    available_Prof1.ProfessorID = context.Professors.ToList().FirstOrDefault().ProfessorID;
            //    context.AvailableProfs.Add(available_Prof1);
            //}
            //context.SaveChanges();

            //{
            //    // Appointment için bir veri ekleyelim
            //    Appointment appointment = new Appointment();
            //    appointment.AppointmentID = 1;
            //    appointment.AssistantID = context.Assistants.ToList().FirstOrDefault().AssistantID;
            //    appointment.AvailableProfR = context.AvailableProfs.ToList().FirstOrDefault();
            //    context.Appointments.Add(appointment);
            //}
            //context.SaveChanges();

            //{
            //    Available_Prof available_Prof2 = new Available_Prof { AvailableProfDateStart = DateTime.Now.AddMinutes(130), AvailableProfDateEnd = DateTime.Now.AddMinutes(250), IsAvailable = true };
            //    available_Prof2.ProfessorID = context.Professors.ToList().FirstOrDefault().ProfessorID;
            //    context.AvailableProfs.Add(available_Prof2);
            //}
            //context.SaveChanges();


            //base.Seed(context);
    //    }
    //}
        
}