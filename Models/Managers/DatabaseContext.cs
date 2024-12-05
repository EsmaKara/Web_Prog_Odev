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
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Available_Prof> AvailableProfs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }

        // constructer metoduna DatabaseCreator sınıfı eklenir ki çalışsın ve veri tabanı yoksa oluşturulsun
        public DatabaseContext() 
        { 
            Database.SetInitializer(new DatabaseCreator());
        }
    }


    // Bu sınıf, veritabanı varlığını kontrol eder ve eğer veritabanı yoksa sınıf tanımlamalarına göre yeni bir veritabanı oluşturur (generic).
    public class DatabaseCreator : CreateDatabaseIfNotExists<DatabaseContext>
    {
        private int NumberOfPerson = 18;
        private int NumberOfProfessor = 3;
        private int NumberOfAssistant = 15;
        public override void InitializeDatabase(DatabaseContext context)
        {
            base.InitializeDatabase(context);
        }

        // database oluşturulduktan sonra çalışır, veri ekleme işlemleri burada yapılır
        // Veriler için rastgele veriler üretebilen FakeData NuGet paketi kullanılır
        protected override void Seed(DatabaseContext context)
        {
            // Department için verileri ekleme
            string[] departmentNameArray = new string[3]
            {
                "Pediatric Emergency",
                "Pediatric Intensive Care Unit (PICU)",
                "Pediatric Hematology and Oncology"
            };
            for (int i = 0; i < departmentNameArray.Length; i++)
            {
                Department department = new Department();
                department.DepartmentName = departmentNameArray[i];
                department.Dep_NumberOfPatients = FakeData.NumberData.GetNumber(0 - 100);
                department.Dep_NumberOfBed = FakeData.NumberData.GetNumber(1 - 50);

                int randomNumber = FakeData.NumberData.GetNumber(1 - 50);
                if (department.Dep_NumberOfBed >= randomNumber)
                    department.Dep_NumberOfBedridden = randomNumber;

                department.Dep_NumberOfEmptyBed = department.Dep_NumberOfBed - department.Dep_NumberOfBedridden;

                context.Departments.Add(department);
            }
            // eklemek için kodlar yazıldı ancak tablolara eklenebilmesi için değişikliklerin kaydedilmesi gerekir
            context.SaveChanges();


            // Person verileri eklenecek, Professor ve Assistant olarak;
            
            List<Department> deps = context.Departments.ToList();
            // Professor için verileri ekleme ve Department ile professor'lerin bağlanması
            foreach (Department d in deps) {
                for (int i = 0; i <= NumberOfProfessor; i++)
                {
                    Professor professor = new Professor();
                    professor.PersonName = FakeData.NameData.GetFirstName();
                    professor.PersonSurname = FakeData.NameData.GetSurname();
                    professor.PersonTel = FakeData.PhoneNumberData.GetPhoneNumber();
                    professor.PersonMail = FakeData.NetworkData.GetEmail();
                    professor.PersonType = "Professor";
                    professor.IsApproved = true;

                    professor.DepartmentR = d;
    
                    context.Persons.Add(professor);
                    context.Professors.Add(professor);
                }
            }
            context.SaveChanges();


            // Assistant için verileri ekleme
            for (int i = 0; i <= NumberOfAssistant; i++)
            {
                Assistant assistant = new Assistant();
                assistant.PersonName = FakeData.NameData.GetFirstName();
                assistant.PersonSurname = FakeData.NameData.GetSurname();
                assistant.PersonTel = FakeData.PhoneNumberData.GetPhoneNumber();
                assistant.PersonMail = FakeData.NetworkData.GetEmail();
                assistant.PersonType = "Assistant";
                assistant.IsApproved = false;

                context.Persons.Add(assistant);
                context.Assistants.Add(assistant);
            }
            context.SaveChanges();


            // veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
            List<Person> people = context.Persons.ToList();
            // Address için verileri ekleme ve Person ile bağlantısı
            foreach (Person someone in people)
            {
                for (int i = 0; i <= NumberOfPerson; i++)
                {
                    Address address = new Address();
                    address.AddressCountry = FakeData.PlaceData.GetCountry();
                    address.AddressDescription = FakeData.PlaceData.GetAddress();

                    address.PersonR = someone;

                    context.Addresses.Add(address);
                }
            }
            context.SaveChanges();

            
            
        }
    }
        
}