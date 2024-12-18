namespace Web_Prog_Odev.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Web_Prog_Odev.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<Web_Prog_Odev.Models.Managers.DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true; // Otomatik migration'ları etkinleştir
            ContextKey = "Web_Prog_Odev.Models.Managers.DatabaseContext";
        }



        protected override void Seed(Web_Prog_Odev.Models.Managers.DatabaseContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.



            int NumberOfProfessor = 3;
            int NumberOfAssistant = 15;

            //database oluşturulduktan sonra çalışır, veri ekleme işlemleri burada yapılır
            // Veriler için rastgele veriler üretebilen FakeData NuGet paketi kullanılır

            Console.WriteLine("Seed metodu çağrıldı."); // Debug için ekleme (kontrol)

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
                department.DepartmentID = i;
                department.DepartmentName = departmentNameArray[i];
                department.Dep_Description = " !? ";
                department.Dep_NumberOfPatients = FakeData.NumberData.GetNumber(100);
                department.Dep_NumberOfBed = FakeData.NumberData.GetNumber(50);

                int randomNumber = FakeData.NumberData.GetNumber(50);
                if (department.Dep_NumberOfBed >= randomNumber)
                    department.Dep_NumberOfBedridden = randomNumber;

                department.Dep_NumberOfEmptyBed = department.Dep_NumberOfBed - department.Dep_NumberOfBedridden;

                context.Departments.Add(department);
            }
            // eklemek için kodlar yazıldı ancak tablolara eklenebilmesi için değişikliklerin kaydedilmesi gerekir
            context.SaveChanges();


            // Professor ve Assistant verileri eklenecek

            List<Department> deps = context.Departments.ToList();
            int pSayac = 1;
            // Professor için verileri ekleme ve Department ile professor'lerin bağlanması
            foreach (Department d in deps)
            {
                for (int i = 0; i <= NumberOfProfessor; i++)
                {
                    Professor professor = new Professor();
                    professor.ProfName = FakeData.NameData.GetFirstName();
                    professor.ProfSurname = FakeData.NameData.GetSurname();
                    professor.ProfTel = FakeData.PhoneNumberData.GetPhoneNumber();
                    professor.ProfMail = FakeData.NetworkData.GetEmail();

                    professor.DepartmentR = d;

                    context.Professors.Add(professor);
                    pSayac += 1;
                }
            }
            context.SaveChanges();


            // Assistant için verileri ekleme
            for (int i = 0; i <= NumberOfAssistant; i++)
            {
                Assistant assistant = new Assistant();
                assistant.AssistName = FakeData.NameData.GetFirstName();
                assistant.AssistSurname = FakeData.NameData.GetSurname();
                assistant.AssistTel = FakeData.PhoneNumberData.GetPhoneNumber();
                assistant.AssistMail = FakeData.NetworkData.GetEmail();

                context.Assistants.Add(assistant);
            }
            context.SaveChanges();



            // veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
            List<Professor> profs = context.Professors.ToList();
            // Address için verileri ekleme ve Person ile bağlantısı
            foreach (Professor p in profs)
            {
                Address address = new Address();
                address.AddressCountry = FakeData.PlaceData.GetCountry();
                address.AddressDescription = FakeData.PlaceData.GetAddress();

                address.ProfessorID = p.ProfessorID;
                address.ProfessorR = p;

                context.Addresses.Add(address);
            }
            context.SaveChanges();
            // veriler artık veri tabanına eklendi bu yüzden verileri çekebiliriz;
            List<Assistant> assists = context.Assistants.ToList();
            // Address için verileri ekleme ve Person ile bağlantısı
            foreach (Assistant a in assists)
            {
                Address address = new Address();
                address.AddressCountry = FakeData.PlaceData.GetCountry();
                address.AddressDescription = FakeData.PlaceData.GetAddress();

                address.AssistantID = a.AssistantID;


                context.Addresses.Add(address);
            }
            context.SaveChanges();



            // Bütün tablolara en az bir veri eklenmesi gerekiyor;;;

            {
                // Shift için bir veri ekleyelim
                Shift shift1 = new Shift { ShiftStart = DateTime.Now, ShiftEnd = DateTime.Now.AddDays(1) };
                shift1.AssistantID = context.Assistants.ToList().FirstOrDefault().AssistantID;
                shift1.DepartmentID = context.Departments.ToList().FirstOrDefault().DepartmentID; ;
                context.Shifts.Add(shift1);
            }
            context.SaveChanges();

            {
                // Emergency için bir veri ekleyelim
                Emergency emergency = new Emergency { EmergencyName = "Help", EmergencyDescription = "Extra help needed!", EmergencyDate = DateTime.Now };
                emergency.DepartmentID = context.Departments.ToList().FirstOrDefault().DepartmentID; ;
                context.Emergencies.Add(emergency);
            }
            context.SaveChanges();

            {
                // Available_Prof için birkaç veri ekleyelim, biri için randevu alınmış olsun
                Available_Prof available_Prof1 = new Available_Prof { AvailableProfDateStart = DateTime.Now, AvailableProfDateEnd = DateTime.Now.AddMinutes(120), IsAvailable = false };
                available_Prof1.ProfessorID = context.Professors.ToList().FirstOrDefault().ProfessorID;
                context.AvailableProfs.Add(available_Prof1);
            }
            context.SaveChanges();

            {
                // Appointment için bir veri ekleyelim
                Appointment appointment = new Appointment();
                appointment.AppointmentID = 1;
                appointment.AssistantID = context.Assistants.ToList().FirstOrDefault().AssistantID;
                appointment.AvailableProfR = context.AvailableProfs.ToList().FirstOrDefault();
                context.Appointments.Add(appointment);
            }
            context.SaveChanges();

            {
                Available_Prof available_Prof2 = new Available_Prof { AvailableProfDateStart = DateTime.Now.AddMinutes(130), AvailableProfDateEnd = DateTime.Now.AddMinutes(250), IsAvailable = true };
                available_Prof2.ProfessorID = context.Professors.ToList().FirstOrDefault().ProfessorID;
                context.AvailableProfs.Add(available_Prof2);
            }
            context.SaveChanges();


            base.Seed(context);
        }


    }
}

