using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace createIdentityTables.Data.Migrations
{
    public partial class AddLastPasswordChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChanged",
                type: "datetime2",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPasswordChanged",
                table: "AspNetUsers");
        }
    }
}
