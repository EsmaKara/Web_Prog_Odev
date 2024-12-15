namespace Web_Prog_Odev.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        AddressDescription = c.String(maxLength: 60),
                        AddressCountry = c.String(nullable: false, maxLength: 60),
                        ProfessorID = c.Int(),
                        AssistantID = c.Int(),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.Assistant", t => t.AssistantID)
                .ForeignKey("dbo.Professor", t => t.ProfessorID)
                .Index(t => t.ProfessorID)
                .Index(t => t.AssistantID);
            
            CreateTable(
                "dbo.Assistant",
                c => new
                    {
                        AssistantID = c.Int(nullable: false, identity: true),
                        AssistName = c.String(maxLength: 30),
                        AssistSurname = c.String(maxLength: 30),
                        AssistTel = c.String(maxLength: 12),
                        AssistMail = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssistantID);
            
            CreateTable(
                "dbo.Appointment",
                c => new
                    {
                        AppointmentID = c.Int(nullable: false, identity: true),
                        AssistantID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AppointmentID)
                .ForeignKey("dbo.Assistant", t => t.AssistantID, cascadeDelete: true)
                .ForeignKey("dbo.Available_Prof", t => t.AppointmentID)
                .Index(t => t.AppointmentID)
                .Index(t => t.AssistantID);
            
            CreateTable(
                "dbo.Available_Prof",
                c => new
                    {
                        AvailableProfID = c.Int(nullable: false, identity: true),
                        AvailableProfDateStart = c.DateTime(nullable: false),
                        AvailableProfDateEnd = c.DateTime(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        ProfessorID = c.Int(nullable: false),
                        AppointmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AvailableProfID)
                .ForeignKey("dbo.Professor", t => t.ProfessorID, cascadeDelete: true)
                .Index(t => t.ProfessorID);
            
            CreateTable(
                "dbo.Professor",
                c => new
                    {
                        ProfessorID = c.Int(nullable: false, identity: true),
                        ProfName = c.String(maxLength: 30),
                        ProfSurname = c.String(maxLength: 30),
                        ProfTel = c.String(maxLength: 12),
                        ProfMail = c.String(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProfessorID)
                .ForeignKey("dbo.Department", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 60),
                        Dep_Description = c.String(nullable: false, maxLength: 400),
                        Dep_NumberOfPatients = c.Int(nullable: false),
                        Dep_NumberOfBed = c.Int(nullable: false),
                        Dep_NumberOfBedridden = c.Int(nullable: false),
                        Dep_NumberOfEmptyBed = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Emergency",
                c => new
                    {
                        EmergencyID = c.Int(nullable: false, identity: true),
                        EmergencyName = c.String(nullable: false, maxLength: 100),
                        EmergencyDescription = c.String(maxLength: 200),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmergencyID)
                .ForeignKey("dbo.Department", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Shift",
                c => new
                    {
                        ShiftID = c.Int(nullable: false, identity: true),
                        ShiftStart = c.DateTime(nullable: false),
                        ShiftEnd = c.DateTime(nullable: false),
                        AssistantID = c.Int(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShiftID)
                .ForeignKey("dbo.Assistant", t => t.AssistantID, cascadeDelete: true)
                .ForeignKey("dbo.Department", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.AssistantID)
                .Index(t => t.DepartmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Address", "ProfessorID", "dbo.Professor");
            DropForeignKey("dbo.Address", "AssistantID", "dbo.Assistant");
            DropForeignKey("dbo.Available_Prof", "ProfessorID", "dbo.Professor");
            DropForeignKey("dbo.Professor", "DepartmentID", "dbo.Department");
            DropForeignKey("dbo.Shift", "DepartmentID", "dbo.Department");
            DropForeignKey("dbo.Shift", "AssistantID", "dbo.Assistant");
            DropForeignKey("dbo.Emergency", "DepartmentID", "dbo.Department");
            DropForeignKey("dbo.Appointment", "AppointmentID", "dbo.Available_Prof");
            DropForeignKey("dbo.Appointment", "AssistantID", "dbo.Assistant");
            DropIndex("dbo.Shift", new[] { "DepartmentID" });
            DropIndex("dbo.Shift", new[] { "AssistantID" });
            DropIndex("dbo.Emergency", new[] { "DepartmentID" });
            DropIndex("dbo.Professor", new[] { "DepartmentID" });
            DropIndex("dbo.Available_Prof", new[] { "ProfessorID" });
            DropIndex("dbo.Appointment", new[] { "AssistantID" });
            DropIndex("dbo.Appointment", new[] { "AppointmentID" });
            DropIndex("dbo.Address", new[] { "AssistantID" });
            DropIndex("dbo.Address", new[] { "ProfessorID" });
            DropTable("dbo.Shift");
            DropTable("dbo.Emergency");
            DropTable("dbo.Department");
            DropTable("dbo.Professor");
            DropTable("dbo.Available_Prof");
            DropTable("dbo.Appointment");
            DropTable("dbo.Assistant");
            DropTable("dbo.Address");
        }
    }
}
