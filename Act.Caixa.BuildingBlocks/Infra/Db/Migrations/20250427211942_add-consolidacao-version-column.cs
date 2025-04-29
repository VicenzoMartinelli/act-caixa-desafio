using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Act.Caixa.BuildingBlocks.Infra.Db.Migrations
{
    /// <inheritdoc />
    public partial class addconsolidacaoversioncolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Consolidacoes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Consolidacoes");
        }
    }
}
