using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XForms.Migrations
{
    /// <inheritdoc />
    public partial class DbContextFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Form_FormId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Questions_QuestionId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Form_AspNetUsers_CreatorId",
                table: "Form");

            migrationBuilder.DropForeignKey(
                name: "FK_Form_Templates_TemplateId",
                table: "Form");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Form",
                table: "Form");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "Form",
                newName: "Forms");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_Form_TemplateId",
                table: "Forms",
                newName: "IX_Forms_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Form_CreatorId",
                table: "Forms",
                newName: "IX_Forms_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_FormId_QuestionId",
                table: "Answers",
                newName: "IX_Answers_FormId_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Forms",
                table: "Forms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Forms_FormId",
                table: "Answers",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AspNetUsers_CreatorId",
                table: "Forms",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Forms_FormId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AspNetUsers_CreatorId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Forms",
                table: "Forms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.RenameTable(
                name: "Forms",
                newName: "Form");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_TemplateId",
                table: "Form",
                newName: "IX_Form_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_CreatorId",
                table: "Form",
                newName: "IX_Form_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answer",
                newName: "IX_Answer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_FormId_QuestionId",
                table: "Answer",
                newName: "IX_Answer_FormId_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Form",
                table: "Form",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Form_FormId",
                table: "Answer",
                column: "FormId",
                principalTable: "Form",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Questions_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Form_AspNetUsers_CreatorId",
                table: "Form",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Form_Templates_TemplateId",
                table: "Form",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
