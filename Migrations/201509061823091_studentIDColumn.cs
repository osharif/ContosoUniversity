namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentIDColumn : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");
            DropPrimaryKey("dbo.Student");
            DropColumn("dbo.Student", "ID");

            // Create  a department for course to point to.
            Sql("INSERT INTO dbo.Student (LastName, FirstName, EnrollmentDate) VALUES ('tempName', 'MidName', GETDATE())");
            //  default value for FK points to department created above.
            AddColumn("dbo.Student", "StudentID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Student", "StudentID");
            AddForeignKey("dbo.Enrollment", "StudentID", "dbo.Student", "StudentID", cascadeDelete: true);
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Student", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");
            DropPrimaryKey("dbo.Student");
            DropColumn("dbo.Student", "StudentID");
            AddPrimaryKey("dbo.Student", "ID");
            AddForeignKey("dbo.Enrollment", "StudentID", "dbo.Student", "ID", cascadeDelete: true);
        }
    }
}
