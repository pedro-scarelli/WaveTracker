using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_users",
                columns: table => new
                {
                    pk_id_user = table.Column<Guid>(type: "uuid", nullable: false),
                    st_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    st_email = table.Column<string>(type: "varchar(100)", nullable: false),
                    st_hashed_password = table.Column<string>(type: "varchar(255)", nullable: false),
                    dt_created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    dt_deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_users", x => x.pk_id_user);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "tb_users",
                column: "st_email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_users");
        }
    }
}
