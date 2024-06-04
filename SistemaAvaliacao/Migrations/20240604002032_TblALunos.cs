using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAvaliacao.Migrations
{
    /// <inheritdoc />
    public partial class TblALunos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Media",
                table: "Aluno",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Media",
                table: "Aluno");
        }
    }
}
