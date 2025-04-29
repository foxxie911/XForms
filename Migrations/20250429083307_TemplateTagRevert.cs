using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XForms.Migrations
{
    /// <inheritdoc />
    public partial class TemplateTagRevert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TemplateTags",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateTags_TemplateId",
                table: "TemplateTags",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags");

            migrationBuilder.DropIndex(
                name: "IX_TemplateTags_TemplateId",
                table: "TemplateTags");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TemplateTags",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags",
                columns: new[] { "TemplateId", "TagId" });
        }
    }
}
