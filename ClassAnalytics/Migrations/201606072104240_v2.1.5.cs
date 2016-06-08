namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v215 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GradeBookModels", "grade", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GradeBookModels", "grade", c => c.Decimal(nullable: true, precision: 18, scale: 2));
        }
    }
}
