using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DanClarkeBlog.Core.Migrations
{
    public partial class AddUniqueConstraintOnTagName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_TagName",
                table: "Tags",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_TagName",
                table: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
