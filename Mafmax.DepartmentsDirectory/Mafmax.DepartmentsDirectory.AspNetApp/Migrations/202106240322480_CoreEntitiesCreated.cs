namespace Mafmax.DepartmentsDirectory.AspNetApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoreEntitiesCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Company_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Department_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .Index(t => t.Department_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.Departments", "Company_Id", "dbo.Companies");
            DropIndex("dbo.Groups", new[] { "Department_Id" });
            DropIndex("dbo.Departments", new[] { "Company_Id" });
            DropTable("dbo.Groups");
            DropTable("dbo.Departments");
            DropTable("dbo.Companies");
        }
    }
}
