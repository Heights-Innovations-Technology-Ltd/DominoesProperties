using System;
using System.IO;
using FluentMigrator;

namespace DominoesProperties.Migrations
{
    [Migration(20240521155430)]
    public class M20240521155430Create_Quartz_Object : Migration
    {
        public override void Up()
        {
            var url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Script", "quartz.sql");
            Execute.Script(url);
        }
        
        public override void Down()
        {
        }
    }
}
